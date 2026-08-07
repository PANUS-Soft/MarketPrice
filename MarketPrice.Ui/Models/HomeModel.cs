using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Models
{
    public partial class CommodityGroupDisplayModel : ObservableObject
    {
        [ObservableProperty] Guid _commodityTypeId;
        [ObservableProperty] string _groupName = string.Empty;
        [ObservableProperty] ObservableCollection<CommodityDisplayModel> _commodities = new();
    }
    
    public partial class CommodityDisplayModel : ObservableObject
    {
        [ObservableProperty] string _nextBid1Location = string.Empty;
        [ObservableProperty] string _nextBid2Location = string.Empty;
        [ObservableProperty] string _nextOffer1Location = string.Empty;
        [ObservableProperty] string _nextOffer2Location = string.Empty;


        [ObservableProperty] Guid commodityTypeId;
        [ObservableProperty] Guid commodityId;
        [ObservableProperty] string name = string.Empty;
        [ObservableProperty] ImageSource? imageUrl;
        [ObservableProperty] string lotSizeDisplay = string.Empty;

        [ObservableProperty] bool isBidImproved;
        [ObservableProperty] bool isOfferImproved;
        [ObservableProperty] bool isBidSoonToExpire;
        [ObservableProperty] bool isOfferSoonToExpire;

        [ObservableProperty] decimal bestBidPrice;
        [ObservableProperty] decimal bestBidQuantity;
        [ObservableProperty] string  bestBidLocation = string.Empty;
        [ObservableProperty] string bestBidDisplay = "No Bid";

        [ObservableProperty] decimal nextBid1;
        [ObservableProperty] decimal nextBid2;

        [ObservableProperty] decimal bestOfferPrice;
        [ObservableProperty] decimal bestOfferQuantity;
        [ObservableProperty] string bestOfferLocation = string.Empty;
        [ObservableProperty] string bestOfferDisplay = "No Offer";

        [ObservableProperty] decimal nextOffer1;
        [ObservableProperty] decimal nextOffer2;

        [ObservableProperty] string nextBid1Display = "-";
        [ObservableProperty] string nextBid2Display = "-";

        [ObservableProperty] string nextOffer1Display = "-";
        [ObservableProperty] string nextOffer2Display = "-";


        [ObservableProperty] string buyerCountDisplay = "x0 Buyer(s)";
        [ObservableProperty] string sellerCountDisplay = "x0 Seller(s)";

        [ObservableProperty] bool _hasBid;
        [ObservableProperty] bool _hasOffer;
    }
}

