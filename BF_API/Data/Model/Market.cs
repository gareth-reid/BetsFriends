using System;
using System.ComponentModel.DataAnnotations.Schema;
using BetfairNG.Data;

namespace BF_API.Data
{
    public class SingleMarket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventId { get; set; }
        public string MarketName { get; set; }
        public string MarketId { get; set; }
        public string EventType { get; set; }
        public string Competition { get; set; }        
        public DateTime? Date { get; set; }
        public DateTime? DateAdded { get; set; }
        public string SelectionValue { get; set; }
        public double? LayPrice { get; set; }
        public double? BackPrice { get; set; }
        public double? AvlToBack { get; set; }
        public double? AvlToLay { get; set; }

public string Status { get; set; }
        public string Selection
        {
            get
            {
                //MATCH_ODDS
                string home;
                string away;
                if (EventName.Contains(" v "))
                {
                    //i.e. Everton v Liverpool
                    home = EventName.Split(" v ")[0].Trim();
                    away = EventName.Split(" v ")[1].Trim();
                }
                else
                {
                    //i.e. Dodgers @ Padres
                    away = EventName.Split("@")[0].Trim();
                    home = EventName.Split("@")[1].Trim();
                }
                if (MarketName == "Moneyline")
                {
                    if (Order == 0)
                    {
                        return home + " " + Handicap;
                    }
                    else
                    {
                        return away + " " + Handicap;
                    }
                }
                else if (MarketName.Contains("Over"))
                {
                    if (Order == 0)
                    {
                        return "Under";
                    }
                    else
                    {
                        return "Over";
                    }
                }
                else if (MarketName == "Match Odds")
                {
                    if (Order == 0)
                    {
                        return home + " Win";
                    }
                    else if (Order == 1)
                    {
                        return away + " Win";
                    }
                    else
                    {
                        return "Draw";
                    }
                } else
                {
                    return "NA";
                }
            }
            set
            {
                SelectionValue = value;
            }
        }
        public long SelectionId { get; set; }
        public double? Handicap { get; set; }
        public int Order { get; set; }

        public double? LastPriceTraded { get; set; }
        public double Price { get; set; }
        public double BackLayRatio { get; set; }

        public SingleMarket()
        {
        }
    }
}
