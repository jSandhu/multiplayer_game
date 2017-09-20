using Common.Inventory;
using System;

namespace Player
{
    public class PlayerInventoryModel : ItemCollection
    {
        public event Action<int> GoldCurrencyUpdated;
        private int _goldCurrency;
        public int GoldCurrency {
            get { return _goldCurrency; }
            set
            {
                _goldCurrency = value;
                if (GoldCurrencyUpdated != null) GoldCurrencyUpdated(_goldCurrency);
            }
        }

        public event Action<int> ExperienceCurrencyUpdated;
        private int _experienceCurrency;
        public int ExperienceCurrency {
            get { return _experienceCurrency; }
            set
            {
                _experienceCurrency = value;
                if (ExperienceCurrencyUpdated != null) ExperienceCurrencyUpdated(_experienceCurrency);
            }
        }
    }
}
