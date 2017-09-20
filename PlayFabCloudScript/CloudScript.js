handlers.GrantMultipleCurrenciesToUser = function (args, context) {
	var currencyTypes = args.currencyTypes;
	var currencyValues = args.currencyValues;
	
	var numCurrencies = currencyTypes.length;
	for (var i = 0; i < numCurrencies; i++)
	{
		 server.AddUserVirtualCurrency({
			PlayFabId: currentPlayerId,
			VirtualCurrency: currencyTypes[i],
			Amount: currencyValues[i]
		});
	}
	
    return { success: true };
};