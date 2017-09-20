using Common.Inventory;
using Common.Inventory.Currency;
using Common.Serialization;

namespace Common.Inventory
{
    public class Bundle : ISerializableModel
    {
        public ItemModel[] ItemModels;
        public CurrencySymbol[] CurrencySymbols;
        public int[] CurrencyValues;

        public Bundle() { }

        public Bundle(ItemModel[] itemModels, CurrencySymbol[] currencySymbols, int[] currencies)
        {
            ItemModels = itemModels;
            CurrencySymbols = currencySymbols;
            CurrencyValues = currencies;
        }

        public object[] ToObjectArray()
        {
            bool hasItems = ItemModels != null && ItemModels.Length > 0;
            bool hasCurrencies = CurrencyValues != null && CurrencyValues.Length > 0;

            int size = 2;
            if (hasItems) size+=2;
            if (hasCurrencies) size+=2;

            int index = 0;
            object[] output = new object[size];
            output[index++] = hasItems;
            output[index++] = hasCurrencies;

            if (hasItems)
            {
                object[] itemTypesAsObjects = new object[ItemModels.Length];
                object[] itemModelsAdObjectArrays = new object[ItemModels.Length];
                for (int i = 0; i < ItemModels.Length; i++)
                {
                    itemTypesAsObjects[i] = ItemModels[i].ItemType;
                    itemModelsAdObjectArrays[i] = ItemModels[i].ToObjectArray();
                }
                output[index++] = itemTypesAsObjects;
                output[index++] = itemModelsAdObjectArrays;
            }

            if (hasCurrencies)
            {
                object[] currencySymbolsAsObjects = new object[CurrencySymbols.Length];
                object[] currencyValuesAsObjects = new object[CurrencyValues.Length];
                for (int c = 0; c < CurrencyValues.Length; c++)
                {
                    currencySymbolsAsObjects[c] = CurrencySymbols[c];
                    currencyValuesAsObjects[c] = CurrencyValues[c];
                }
                output[index++] = currencySymbolsAsObjects;
                output[index++] = currencyValuesAsObjects;
            }

            return output;
        }

        public void FromObjectArray(object[] properties)
        {
            int index = 0;
            bool hasItems = (bool)properties[index++];
            bool hasCurrencies = (bool)properties[index++];

            if (hasItems)
            {
                object[] itemTypesAsObjects = properties[index++] as object[];
                object[] itemsAsObjectArrays = properties[index++] as object[];
                ItemModels = new ItemModel[itemsAsObjectArrays.Length];
                for (int i = 0; i < ItemModels.Length; i++)
                {
                    ItemModels[i] = ItemModelFactory.GetItemModelFromItemType((ItemType)itemTypesAsObjects[i]);
                    ItemModels[i].FromObjectArray(itemsAsObjectArrays[i] as object[]);
                }
            }

            if (hasCurrencies)
            {
                object[] currencySymbolsAsObjects = properties[index++] as object[];
                object[] currencyValuesAsObjects = properties[index++] as object[];
                CurrencySymbols = new CurrencySymbol[currencySymbolsAsObjects.Length];
                CurrencyValues = new int[currencyValuesAsObjects.Length];
                for(int c = 0; c < CurrencyValues.Length; c++)
                {
                    CurrencySymbols[c] = (CurrencySymbol)currencySymbolsAsObjects[c];
                    CurrencyValues[c] = (int)currencyValuesAsObjects[c];
                }
            }
        }
    }
}
