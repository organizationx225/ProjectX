import yfinance as yf
import pandas as pd
import numpy as np
from statsmodels.tsa.arima.model import ARIMA
from statsmodels.tsa.holtwinters import ExponentialSmoothing
import statsmodels.api as sm

def fetch_asset_data(ticker):
    """
    Fetch historical asset prices using yfinance.
    The function returns a DataFrame with the 'Close' price.
    """
    asset = yf.Ticker(ticker)
    # Fetch approximately 15 years of data for a robust dataset.
    df = asset.history(period="15y")
    df = df[['Close']]
    df.dropna(inplace=True)
    return df

def resample_yearly(df):
    """
    Resample daily data to yearly frequency using the last available closing price.
    """
    yearly = df.resample('Y').last()
    return yearly

def forecast_arima(yearly_data, steps=10):
    """
    Forecast using an ARIMA(1,1,1) model.
    Returns the forecasted mean and a DataFrame with 95% confidence intervals.
    """
    model = ARIMA(yearly_data['Close'], order=(1, 1, 1))
    model_fit = model.fit()
    forecast_result = model_fit.get_forecast(steps=steps)
    forecast = forecast_result.predicted_mean
    conf_int = forecast_result.conf_int(alpha=0.05)
    return forecast, conf_int

def forecast_exp_smoothing(yearly_data, steps=10):
    """
    Forecast using Exponential Smoothing (Holt-Winters with an additive trend).
    Since this method does not directly provide forecast intervals,
    we compute approximate 95% confidence intervals using the in-sample residuals.
    """
    model = ExponentialSmoothing(yearly_data['Close'], trend='add', seasonal=None)
    model_fit = model.fit(optimized=True)
    forecast = model_fit.forecast(steps)
    
    # Compute residuals and standard deviation of the residuals.
    residuals = yearly_data['Close'] - model_fit.fittedvalues
    sigma = np.std(residuals)
    
    # For each forecast horizon, approximate the forecast error variance as sigma * sqrt(h)
    lower_ci = []
    upper_ci = []
    for h in range(1, steps+1):
        err = 1.96 * sigma * np.sqrt(h)
        lower_ci.append(forecast.iloc[h-1] - err)
        upper_ci.append(forecast.iloc[h-1] + err)
    
    conf_int = pd.DataFrame({'lower': lower_ci, 'upper': upper_ci}, index=forecast.index)
    return forecast, conf_int

def forecast_linear_regression(yearly_data, steps=10):
    """
    Forecast using a simple linear regression on time.
    A numeric time index is created and OLS is used to fit the trend.
    Forecast intervals are generated using the OLS prediction summary.
    """
    df_lr = yearly_data.copy().reset_index()
    # Rename the date column to "Date" (if not already named)
    if 'Date' not in df_lr.columns:
        df_lr.rename(columns={df_lr.columns[0]: "Date"}, inplace=True)
    # Create a numeric time variable (t = 0, 1, 2, ...)
    df_lr['t'] = np.arange(len(df_lr))
    X = sm.add_constant(df_lr['t'])
    y = df_lr['Close']
    model = sm.OLS(y, X).fit()
    
    # Prepare future time values.
    last_t = df_lr['t'].iloc[-1]
    t_future = np.arange(last_t + 1, last_t + steps + 1)
    X_future = sm.add_constant(t_future)
    prediction = model.get_prediction(X_future)
    prediction_summary = prediction.summary_frame(alpha=0.05)
    
    forecast = prediction_summary['mean']
    conf_int = prediction_summary[['obs_ci_lower', 'obs_ci_upper']]
    
    # Create a DatetimeIndex for the forecast based on yearly frequency.
    last_year = df_lr['Date'].iloc[-1]
    forecast_index = pd.date_range(start=last_year + pd.DateOffset(years=1), periods=steps, freq='Y')
    forecast.index = forecast_index
    conf_int.index = forecast_index
    return forecast, conf_int

def build_forecast_df(forecast, conf_int, asset_amount, model_name, ticker):
    """
    Build a DataFrame summarizing the forecasted asset prices, confidence intervals,
    and the estimated asset values for each forecast year.
    """
    df_model = pd.DataFrame({
        'Ticker': ticker,
        'Year': forecast.index.year,
        'Forecast Price (USD/unit)': forecast.values,
        'Lower CI (USD/unit)': conf_int.iloc[:, 0].values,
        'Upper CI (USD/unit)': conf_int.iloc[:, 1].values,
    })
    df_model['Asset Value (USD)'] = df_model['Forecast Price (USD/unit)'] * asset_amount
    df_model['Lower Asset Value (USD)'] = df_model['Lower CI (USD/unit)'] * asset_amount
    df_model['Upper Asset Value (USD)'] = df_model['Upper CI (USD/unit)'] * asset_amount
    df_model['Model'] = model_name
    return df_model

def forecast_for_ticker(ticker, asset_amount, steps=10):
    """
    For a given ticker and asset amount, fetch data, resample, forecast using
    three models, and combine the results into one DataFrame.
    """
    print(f"\nProcessing ticker: {ticker}")
    try:
        df = fetch_asset_data(ticker)
    except Exception as e:
        print(f"Error fetching data for {ticker}: {e}")
        return None

    # Resample to yearly data.
    yearly = resample_yearly(df)
    if yearly.empty:
        print(f"No yearly data found for {ticker}.")
        return None

    # Ensure the forecast index is a DatetimeIndex for all methods.
    last_year = yearly.index[-1]
    forecast_index = pd.date_range(start=last_year + pd.DateOffset(years=1), periods=steps, freq='Y')
    
    # ARIMA Forecast
    try:
        arima_forecast, arima_conf = forecast_arima(yearly, steps=steps)
        if not isinstance(arima_forecast.index, pd.DatetimeIndex):
            arima_forecast.index = forecast_index
            arima_conf.index = forecast_index
        df_arima = build_forecast_df(arima_forecast, arima_conf, asset_amount, "ARIMA", ticker)
    except Exception as e:
        print(f"ARIMA model failed for {ticker}: {e}")
        df_arima = pd.DataFrame()

    # Exponential Smoothing Forecast
    try:
        exp_forecast, exp_conf = forecast_exp_smoothing(yearly, steps=steps)
        if not isinstance(exp_forecast.index, pd.DatetimeIndex):
            exp_forecast.index = forecast_index
            exp_conf.index = forecast_index
        df_exp = build_forecast_df(exp_forecast, exp_conf, asset_amount, "Exponential Smoothing", ticker)
    except Exception as e:
        print(f"Exponential Smoothing model failed for {ticker}: {e}")
        df_exp = pd.DataFrame()

    # Linear Regression Forecast
    try:
        lr_forecast, lr_conf = forecast_linear_regression(yearly, steps=steps)
        df_lr = build_forecast_df(lr_forecast, lr_conf, asset_amount, "Linear Regression", ticker)
    except Exception as e:
        print(f"Linear Regression model failed for {ticker}: {e}")
        df_lr = pd.DataFrame()

    # Combine the models’ forecasts.
    forecast_all = pd.concat([df_arima, df_exp, df_lr], ignore_index=True)
    return forecast_all

def main():
    # Get a list of ticker symbols from the user.
    tickers_input = input("Enter ticker(s) separated by commas (e.g., GC=F, ^GSPC, SI=F): ")
    tickers = [ticker.strip() for ticker in tickers_input.split(',') if ticker.strip()]
    
    if not tickers:
        print("No tickers provided. Exiting.")
        return

    # For each ticker, ask for the asset amount (e.g. ounces, shares)
    asset_amounts = {}
    for ticker in tickers:
        try:
            amt = float(input(f"Enter asset amount (in units) for {ticker}: "))
            asset_amounts[ticker] = amt
        except ValueError:
            print(f"Invalid input for {ticker}. Using asset amount = 1.")
            asset_amounts[ticker] = 1.0

    steps = 10  # Forecast horizon (10 years)
    all_forecasts = []

    # Process each ticker and compile the forecast results.
    for ticker in tickers:
        forecast_df = forecast_for_ticker(ticker, asset_amounts[ticker], steps=steps)
        if forecast_df is not None and not forecast_df.empty:
            all_forecasts.append(forecast_df)

    if all_forecasts:
        combined_forecast = pd.concat(all_forecasts, ignore_index=True)
        # Organize the output columns.
        combined_forecast = combined_forecast[['Ticker', 'Model', 'Year',
                                               'Forecast Price (USD/unit)',
                                               'Lower CI (USD/unit)',
                                               'Upper CI (USD/unit)',
                                               'Asset Value (USD)',
                                               'Lower Asset Value (USD)',
                                               'Upper Asset Value (USD)']]
        print("\nForecast for the Next 10 Years using Different Models:")
        print(combined_forecast.to_string(index=False))
        print("\nEstimation Confidence:")
        print("Each model produces its own forecast along with 95% confidence intervals based on its methodology.")
        print("Wide intervals in one model might indicate sensitivity to model assumptions or limitations in the historical data.")
    else:
        print("No forecasts to display.")

if __name__ == "__main__":
    main()
