(function (global) {
    global.buildChart = (type) => {
        var k = new TradingView.widget(
            {
                "autosize": true,
                "symbol": "BINANCE:BTCUSDT",
                "interval": "1",
                "timezone": "Africa/Johannesburg",
                "theme": "dark",
                "style": "2",
                "locale": "en",
                "toolbar_bg": "#f1f3f6",
                "enable_publishing": false,
                "hide_top_toolbar": true,
                "save_image": false,
                "container_id": "tradingview_bd4bd"
            }
        );

        global.tradingview_bd4bd = k;
    }
})(window);










