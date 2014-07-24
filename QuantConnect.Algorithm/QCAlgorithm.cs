﻿/*
* QUANTCONNECT.COM - 
* QC.Algorithm - Base Class for Algorithm.
*/

/**********************************************************
* USING NAMESPACES
**********************************************************/
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using QuantConnect.Securities;
using QuantConnect.Models;

namespace QuantConnect 
{
    /******************************************************** 
    * CLASS DEFINITIONS
    *********************************************************/
    /// <summary>
    /// QC Algorithm Base Class - Handle the basic requirement of a trading algorithm, 
    /// allowing user to focus on event methods.
    /// </summary>
    public class QCAlgorithm : MarshalByRefObject, IAlgorithm 
    {
        /******************************************************** 
        * CLASS PRIVATE VARIABLES
        *********************************************************/
        private DateTime _time = new DateTime();
        private DateTime _startDate = new DateTime(2000, 01, 01);   //Default start and end dates.
        private DateTime _endDate = DateTime.Today.AddDays(-1);     //Default end to yesterday
        private RunMode _runMode = RunMode.Automatic;
        private bool _locked = false;
        private string _simulationId = "";
        private bool _quit = false;
        private List<string> _debugMessages = new List<string>();
        private List<string> _logMessages = new List<string>();
        private List<string> _errorMessages = new List<string>();
        private Dictionary<string, Chart> _charts = new Dictionary<string, Chart>();

        public Console Console = new Console(null);

        //Error tracking to avoid message flooding:
        private string previousDebugMessage = "";
        private bool sentNoDataError = false;



        /******************************************************** 
        * CLASS CONSTRUCTOR
        *********************************************************/
        /// <summary>
        /// Initialise the Algorithm
        /// </summary>
        public QCAlgorithm()
        {
            //Initialise the Algorithm Helper Classes:
            //- Note - ideally these wouldn't be here, but because of the DLL we need to make the classes shared across 
            //  the Worker & Algorithm, limiting ability to do anything else.
            Securities = new SecurityManager();
            Transactions = new SecurityTransactionManager(Securities);
            Portfolio = new SecurityPortfolioManager(Securities, Transactions);

            //Initialise Data Manager 
            SubscriptionManager = new SubscriptionManager();

            //Initialise Algorithm RunMode to Automatic:
            _runMode = RunMode.Automatic;

            //Initialise to unlocked:
            _locked = false;

            //Initialise Start and End Dates:
            _startDate = new DateTime();
            _endDate = new DateTime();
            _charts = new Dictionary<string, Chart>();

            //Init Console Override:
            Console = new Console(this);
        }


        /******************************************************** 
        * CLASS PUBLIC VARIABLES
        *********************************************************/
        /// <summary>
        /// Security Object Collection
        /// </summary>
        public SecurityManager Securities { 
            get; 
            set; 
        }

        /// <summary>
        /// Portfolio Adaptor/Wrapper: Easy access to securities holding properties:
        /// </summary>
        public SecurityPortfolioManager Portfolio { 
            get; 
            set; 
        }

        /// <summary>
        /// Transaction Manager - Process transaction fills and order management.
        /// </summary>
        public SecurityTransactionManager Transactions { 
            get; 
            set; 
        }

        /// <summary>
        /// Generic Data Manager - Required for compiling all data feeds in order,
        /// and passing them into algorithm event methods.
        /// </summary>
        public SubscriptionManager SubscriptionManager { 
            get; 
            set; 
        }

        /// <summary>
        /// Set a public name for the algorithm.
        /// </summary>
        public string Name 
        {
            get;
            set;
        }

        /// <summary>
        /// Get the current date/time.
        /// </summary>
        public DateTime Time 
        {
            get 
            {
                return _time;
            }
        }

        /// <summary>
        /// Get requested simulation start date set with SetStartDate()
        /// </summary>
        public DateTime StartDate 
        {
            get 
            {
                return _startDate;
            }
        }

        /// <summary>
        /// Get requested simulation end date set with SetEndDate()
        /// </summary>
        public DateTime EndDate 
        {
            get 
            {
                return _endDate;
            }
        }

        /// <summary>
        /// Simulation Id for this Backtest
        /// </summary>
        public string SimulationId 
        {
            get 
            {
                return _simulationId;
            }
        }

        /// <summary>
        /// Accessor for Filled Orders:
        /// </summary>
        public ConcurrentDictionary<int, Order> Orders 
        {
            get 
            {
                return Transactions.Orders;
            }
        }

        /// <summary>
        /// Simulation Server setup RunMode for the Algorithm: Automatic, Parallel or Series.
        /// </summary>
        public RunMode RunMode 
        {
            get 
            {
                return _runMode;
            }
        }

        /// <summary>
        /// Check if the algorithm is locked from any further init changes.
        /// </summary>
        public bool Locked 
        {
            get 
            {
                return _locked;
            }
        }

        /// <summary>
        /// Get the debug messages from inner list
        /// </summary>
        public List<string> DebugMessages
        {
            get 
            {
                return _debugMessages;
            }
            set 
            {
                _debugMessages = value;
            }
        }

        /// <summary>
        /// Downloadable large scale messaging systems
        /// </summary>
        public List<string> LogMessages 
        {
            get 
            {
                return _logMessages;
            }
            set 
            {
                _logMessages = value;
            }
        }

        /// <summary>
        /// Catchable Error List.
        /// </summary>
        public List<string> ErrorMessages
        {
            get
            {
                return _errorMessages;
            }
            set
            {
                _errorMessages = value;
            }
        }

        /******************************************************** 
        * CLASS METHODS
        *********************************************************/
        /// <summary>
        /// Initialise the data and resolution requiredv 
        /// </summary>
        public virtual void Initialize() 
        {
            //Setup Required Data
            throw new NotImplementedException("Please override the Intitialize() method");
        }

        /// <summary>
        /// DEPRECATED - v1.0 TRADEBAR EVENT HANDLER
        /// New data routine: handle new data packets. Algorithm starts here..
        /// </summary>
        /// <param name="symbols">Dictionary of MarketData Objects</param>
        public virtual void OnTradeBar(Dictionary<string, TradeBar> data)
        {
            //Algorithm Implementation
            //throw new NotImplementedException("OnTradeBar has been made obsolete. Please use OnData(TradeBars data) instead.");
        }

        /// <summary>
        /// DEPRECATED - v1.0 TICK EVENT HANDLER
        /// Handle a new incoming Tick Packet:
        /// </summary>
        /// <param name="ticks">Ticks arriving at the same moment come in a list. Because the "tick" data is actually list ordered within a second, you can get lots of ticks at once.</param>
        public virtual void OnTick(Dictionary<string, List<Tick>> data)
        {
            //Algorithm Implementation
            //throw new NotImplementedException("OnTick has been made obsolete. Please use OnData(Ticks data) instead.");
        }

        /// <summary>
        /// v2.0 TRADEBAR EVENT HANDLER: (Pattern)
        /// Basic template for user to override when requesting tradebar data.
        /// </summary>
        /// <param name="data"></param>
        //public void OnData(TradeBars data)
        //{
        //
        //}

        /// <summary>
        /// v2.0 TICK EVENT HANDLER: (Pattern)
        /// Basic template for user to override when requesting tick data.
        /// </summary>
        /// <param name="data">List of Tick Data</param>
        //public void OnData(Ticks data)
        //{
        //
        //}

        /// <summary>
        /// Call this method at the end of the algorithm day
        /// </summary>
        public virtual void OnEndOfDay() 
        {
            
        }

        /// <summary>
        /// Call this at the end of the algorithm running.
        /// </summary>
        public virtual void OnEndOfAlgorithm() 
        { 
            
        }

        /// <summary>
        /// Plot using a default chart name,
        /// </summary>
        /// <param name="series">Name of the plot series</param>
        /// <param name="value">Value to plot</param>
        public void Plot(string series, decimal value)
        {
            this.Plot("Custom", series, value);
        }

        /// <summary>
        /// Alias of Plot();
        /// </summary>
        /// <param name="series">Name of the series</param>
        /// <param name="value">Value of the series plot</param>
        public virtual void Record(string series, decimal value)
        {
            this.Plot("Custom", series, value);
        }

        /// <summary>
        /// Add this chart to our collection
        /// </summary>
        /// <param name="name">string name of our chart</param>
        /// <param name="chart">chart object</param>
        public void AddChart(Chart chart) 
        {
            if (!_charts.ContainsKey(chart.Name)) 
            {
                _charts.Add(chart.Name, chart);
            }
        }

        /// <summary>
        /// Plot a value to a chart. If chart does not exist, create it:
        /// </summary>
        /// <param name="chart">Chart name</param>
        /// <param name="series">Series name</param>
        /// <param name="value">Value of the point</param>
        public void Plot(string chart, string series, decimal value) 
        {
            //Ignore the reserved chart names:
            if ((chart == "Strategy Equity" && series == "Equity") || (chart == "Daily Performance"))
            {
                throw new Exception("Algorithm.Plot(): 'Equity' and 'Performance' are reserved chart names create for all backtests.");
            }

            // If we don't have the chart, create it:
            if (!_charts.ContainsKey(chart))
            {
                _charts.Add(chart, new Chart(chart)); 
            }

            if (!_charts[chart].Series.ContainsKey(series)) 
            {
                //Number of series in total.
                int seriesCount = (from x in _charts.Values select x.Series.Count).Sum();

                if (seriesCount > 10)
                {
                    Error("Exceeded maximum series count: Each backtest can have up to 10 series in total.");
                    return;
                }

                //If we don't have the series, create it:
                _charts[chart].AddSeries(new ChartSeries(series));
            }

            if (_charts[chart].Series[series].Values.Count < 4000)
            {
                _charts[chart].Series[series].AddPoint(Time, value);
            }
            else 
            {
                Debug("Exceeded maximum points per chart, data skipped.");
            }
        }

        /// <summary>
        /// Set the current datetime frontier: the most forward looking tick so far. This is used by backend to advance time. Do not modify
        /// </summary>
        /// <param name="frontier">Current datetime.</param>
        public void SetDateTime(DateTime frontier) 
        {
            this._time = frontier;
        }

        /// <summary>
        /// Set the RunMode for the Servers. If you are running an overnight algorithm, you must select series.
        /// Automatic will analyse the selected data, and if you selected only minute data we'll select series for you.
        /// </summary>
        /// <param name="mode">Enum RunMode with options Series, Parallel or Automatic. Automatic scans your requested symbols and resolutions and makes a decision on the fastest analysis</param>
        public void SetRunMode(RunMode mode) 
        {
            if (!Locked) 
            {
                this._runMode = RunMode.Series;
                if (mode == RunMode.Parallel) 
                {
                    throw new Exception("Algorithm.SetRunMode(): RunMode-Parallel Type has been deprecated. Please use series analysis instead");
                }
            }
            else 
            {
                throw new Exception("Algorithm.SetRunMode(): Cannot change run mode after algorithm initialized.");
            }
        }

        /// <summary>
        /// Set the requested balance to launch this algorithm
        /// </summary>
        /// <param name="startingCash">Minimum required cash</param>
        public void SetCash(decimal startingCash) 
        {
            if (!Locked) 
            {
                Portfolio.SetCash(startingCash);
            }
            else 
            {
                throw new Exception("Algorithm.SetCash(): Cannot change cash available after algorithm initialized.");
            }
        }

        /// <summary>
        /// Wrapper for SetStartDate(DateTime). Set the start date for simulation.
        /// Must be less than end date.
        /// </summary>
        /// <param name="year">int year</param>
        /// <param name="month">int month</param>
        /// <param name="day">int day</param>
        public void SetStartDate(int year, int month, int day) 
        {
            try 
            {
                this.SetStartDate(new DateTime(year, month, day));
            } 
            catch (Exception err) 
            {
                throw new Exception("Date Invalid: " + err.Message);
            }
        }

        /// <summary>
        /// Wrapper for SetEndDate(datetime). Set the end simulation date. 
        /// </summary>
        /// <param name="year">int year</param>
        /// <param name="month">int month</param>
        /// <param name="day">int day</param>
        public void SetEndDate(int year, int month, int day) 
        {
            try 
            {
                this.SetEndDate(new DateTime(year, month, day));
            } 
            catch (Exception err) 
            {
                throw new Exception("Date Invalid: " + err.Message);
            }
        }

        /// <summary>
        /// Wrapper for SetEndDate(datetime). Set the end simulation date. 
        /// </summary>
        /// <param name="year">int year</param>
        /// <param name="month">int month</param>
        /// <param name="day">int day</param>
        public void SetSimulationId(string simulationId)
        {
            _simulationId = simulationId;
        }

        /// <summary>
        /// Set the start date for the simulation 
        /// Must be less than end date and within data available
        /// </summary>
        /// <param name="start">Datetime start date</param>
        public void SetStartDate(DateTime start) 
        { 
            //Validate the start date:
            //1. Check range;
            if (start < (new DateTime(1998, 01, 01))) 
            {
                throw new Exception("Please select data between January 1st, 1998 to July 31st, 2012.");
            }

            //2. Check end date greater:
            if (_endDate != new DateTime()) 
            {
                if (start > _endDate) 
                {
                    throw new Exception("Please select start date less than end date.");
                }
            }

            //3. Check not locked already:
            if (!Locked) 
            {
                this._startDate = start;
            } 
            else
            {
                throw new Exception("Algorithm.SetStartDate(): Cannot change start date after algorithm initialized.");
            }
        }

        /// <summary>
        /// Set the end date for a simulation. 
        /// Must be greater than the start date
        /// </summary>
        /// <param name="end"></param>
        public void SetEndDate(DateTime end) 
        { 
            //Validate:
            //1. Check Range:
            if (end > DateTime.Now.Date.AddDays(-1)) 
            {
                throw new Exception("Please select data from between January 1st, 1998 to until yesterday.");
            }

            //2. Check start date less:
            if (_startDate != new DateTime()) 
            {
                if (end < _startDate) 
                {
                    throw new Exception("Please select end date greater than start date.");
                }
            }

            //3. Check not locked already:
            if (!Locked) 
            {
                this._endDate = end;
            }
            else 
            {
                throw new Exception("Algorithm.SetEndDate(): Cannot change end date after algorithm initialized.");
            }
        }

        /// <summary>
        /// Lock the algorithm initialization to avoid messing with cash and data streams.
        /// </summary>
        public void SetLocked() 
        {
            this._locked = true;
        }

        /// <summary>
        /// Get the chart updates: fetch the recent points added and return for dynamic plotting.
        /// </summary>
        /// <returns></returns>
        public List<Chart> GetChartUpdates() 
        {
            List<Chart> _updates = new List<Chart>();
            foreach (Chart _chart in _charts.Values) {
                _updates.Add(_chart.GetUpdates());
            }
            return _updates;
        }

        /// <summary>
        /// Add specified data to required list. QC will funnel this data to the handle data routine.
        /// This is a backwards compatibility wrapper function.
        /// </summary>
        /// <param name="securityType">MarketType Type: Equity, Commodity, Future or FOREX</param>
        /// <param name="symbol">Symbol Reference for the MarketType</param>
        /// <param name="resolution">Resolution of the Data Required</param>
        /// <param name="fillDataForward">When no data available on a tradebar, return the last data that was generated</param>
        public void AddSecurity(SecurityType securityType, string symbol, Resolution resolution = Resolution.Minute, bool fillDataForward = true, bool extendedMarketHours = false)
        {
            AddSecurity(securityType, symbol, resolution, fillDataForward, 0, extendedMarketHours);
        }

        /// <summary>
        /// Add specified data to required list. QC will funnel this data to the handle data routine.
        /// </summary>
        /// <param name="securityType">MarketType Type: Equity, Commodity, Future or FOREX</param>
        /// <param name="symbol">Symbol Reference for the MarketType</param>
        /// <param name="resolution">Resolution of the Data Required</param>
        /// <param name="fillDataForward">When no data available on a tradebar, return the last data that was generated</param>
        /// <param name="leverage">Custom leverage per security</param>
        public void AddSecurity(SecurityType securityType, string symbol, Resolution resolution, bool fillDataForward, decimal leverage, bool extendedMarketHours) 
        {
            try
            {
                if (!_locked) 
                {
                    symbol = symbol.ToUpper();
                    //If it hasn't been set, use some defaults based on the portfolio type:
                    if (leverage <= 0) 
                    {
                        switch (securityType) 
                        {
                            case SecurityType.Equity:
                                leverage = 1;   //RegT = 2 or 4.
                                break;
                            case SecurityType.Forex:
                                leverage = 50;
                                break;
                        }
                    }

                    //Add the symbol to Data Manager -- generate unified data streams for algorithm events
                    SubscriptionManager.Add(securityType, symbol, resolution, fillDataForward, extendedMarketHours);
                    //Add the symbol to Securities Manager -- manage collection of portfolio entities for easy access.
                    Securities.Add(symbol, securityType, resolution, fillDataForward, leverage, extendedMarketHours, useQuantConnectData: true);
                }
                else 
                {
                    throw new Exception("Algorithm.AddSecurity(): Cannot add another security after algorithm running.");
                }
            }
            catch (Exception err) 
            {
                Error("Algorithm.AddSecurity(): " + err.Message);
            }
        }


        /// <summary>
        /// Add a new user defined data source, requiring only the minimum config options:
        /// </summary>
        /// <param name="type">typeof(Type) data</param>
        /// <param name="symbol">Key/Symbol for data</param>
        public void AddData<T>(string symbol, Resolution resolution = Resolution.Second) 
        {
            if (!_locked)
            {
                //Add this to the data-feed subscriptions
                SubscriptionManager.Add(typeof(T), SecurityType.Base, symbol, resolution, fillDataForward:false, extendedMarketHours:true);

                //Add this new generic data as a tradeable security: 
                // Defaults:extended market hours"      = true because we want events 24 hours, 
                //          fillforward                 = false because only want to trigger when there's new custom data.
                //          leverage                    = 1 because no leverage on nonmarket data?
                Securities.Add(symbol, SecurityType.Base, resolution, fillDataForward: false, leverage:1, extendedMarketHours:true, useQuantConnectData:false);
            }
        }

        /// <summary>
        /// Submit a new order for quantity of symbol using type order.
        /// </summary>
        /// <param name="type">Buy/Sell Limit or Market Order Type.</param>
        /// <param name="symbol">Symbol of the MarketType Required.</param>
        /// <param name="quantity">Number of shares to request.</param>
        public int Order(string symbol, int quantity, OrderType type = OrderType.Market)
        {
            //Add an order to the transacion manager class:
            int orderId = -1;
            decimal price = 0;
            string orderRejected = "Order Rejected at " + Time.ToShortDateString() + " " + Time.ToShortTimeString() + ": ";

            //Internals use upper case symbols.
            symbol = symbol.ToUpper();

            //Ordering 0 is useless.
            if (quantity == 0) 
            {
                return orderId;
            }

            //If we're not tracking this symbol: throw error:
            if (!Securities.ContainsKey(symbol) && !sentNoDataError)
            {
                sentNoDataError = true;
                Debug(orderRejected + "You haven't requested " + symbol + " data. Add this with AddSecurity() in the Initialize() Method.");
            }

            //Set a temporary price for validating order for market orders:
            price = Securities[symbol].Price;

            try
            {
                orderId = Transactions.AddOrder(new Order(symbol, quantity, type, Time, price));

                if (orderId < 0) 
                { 
                    //Order failed validaity checks and was rejected:
                    Debug(orderRejected + OrderErrors.ErrorTypes[orderId]);
                }
            }
            catch (Exception err) 
            {
                Error("Algorithm.Order(): Error sending order. " + err.Message);
            }
            return orderId;
        }

        /// <summary>
        /// Liquidate all holdings. Called at the end of day for tick-strategies.
        /// </summary>
        /// <returns>Array of order ids for liquidated symbols</returns>
        public List<int> Liquidate(string symbolToLiquidate = "")
        {
            int quantity = 0;
            List<int> orderIdList = new List<int>();

            symbolToLiquidate = symbolToLiquidate.ToUpper();

            foreach (string symbol in Securities.Keys) 
            {
                //Send market order to liquidate if 1, we have stock, 2, symbol matches.
                if (Portfolio[symbol].HoldStock && (symbol == symbolToLiquidate || symbolToLiquidate == "")) 
                {
                    
                    if (Portfolio[symbol].IsLong)
                    {
                        quantity = -Portfolio[symbol].Quantity;
                    }
                    else
                    {
                        quantity = Math.Abs(Portfolio[symbol].Quantity);
                    }
                    //Liquidate at market price.
                    orderIdList.Add(Transactions.AddOrder(new Order(symbol, quantity, OrderType.Market, Time, Securities[symbol].Price)));
                }
            }
            return orderIdList;
        }

        /// <summary>
        /// Send a debug message to the console:
        /// </summary>
        /// <param name="message">Message to send to debug console</param>
        public void Debug(string message)
        {
            if (message == "" || previousDebugMessage == message) return;
            _debugMessages.Add(message);
            previousDebugMessage = message;
        }

        /// <summary>
        /// Added another method for logging if user guessed.
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message) 
        {
            if (message == "") return;
            _logMessages.Add(message);
        }

        /// <summary>
        /// Send Error Message to the Console.
        /// </summary>
        /// <param name="message">Message to display in errors grid</param>
        public void Error(string message)
        {
            if (message == "") return;
            _errorMessages.Add(message);
            Debug("SimulationId:(" + _simulationId + ") Error: " + message);
        }

        /// <summary>
        /// Terminate the algorithm on exiting the current event processor. 
        /// If have holdings at the end of the algorithm/day they will be liquidated at market prices.
        /// If running a series analysis this command skips the current day (and doesn't liquidate).
        /// </summary>
        /// <param name="message">Exit message</param>
        public void Quit(string message = "") 
        {
            Debug("Quit(): " + message);
            _quit = true;
        }

        /// <summary>
        /// Check if the Quit Flag is Set:
        /// </summary>
        public void SetQuit(bool quit) 
        {
            _quit = quit;
        }

        /// <summary>
        /// Get the quit flag state.
        /// </summary>
        /// <returns>Boolean true if set to quit event loop.</returns>
        public bool GetQuit() 
        {
            return _quit;
        }

    } // End Algorithm Template


    /// <summary>
    /// Helper class to override default behaviour of Console.WriteLine(); This will force the write line messages to appear in the browser console.
    /// </summary>
    public class Console
    {
        QCAlgorithm algorithmNamespace;

        /// <summary>
        /// Initialiser for Console Override
        /// </summary>
        /// <param name="algorithmNamespace">Algorithm Debug Function Access</param>
        public Console(QCAlgorithm algorithmNamespace)
        {
            this.algorithmNamespace = algorithmNamespace;
        }

        /// <summary>
        /// Write a line to the console in the browser
        /// </summary>
        /// <param name="consoleMessage">String message to send.</param>
        public void WriteLine(string consoleMessage)
        {
            algorithmNamespace.Debug(consoleMessage);
        }

        /// <summary>
        /// Write a line to the console in the browser
        /// </summary>
        /// <param name="consoleMessage">String message to send.</param>
        public void Write(string consoleMessage)
        {
            algorithmNamespace.Debug(consoleMessage);
        }
    }

} // End QC Namespace
