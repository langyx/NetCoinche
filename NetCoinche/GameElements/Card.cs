using System.Collections.Generic;

namespace NetCoinche
{
    public class Card
    {
        private CardFamily familyCard;
        private CardName familyName;

        private List<Card> cards;

        public Card(CardFamily familyCard, CardName familyName)
        {
            this.familyCard = familyCard;
            this.familyName = familyName;
        }

        public CardFamily FamilyCard
        {
            get => familyCard;
            set => familyCard = value;
        }

        public CardName FamilyName
        {
            get => familyName;
            set => familyName = value;
        }
        
        public void removecard(Card card)
        {
            if (cards.Contains(card))
                cards.Remove(card);
        }
    }
}