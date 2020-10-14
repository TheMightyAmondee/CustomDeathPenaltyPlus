using System;
using StardewModdingAPI;


namespace SmallerDeathPenalty
{
    /// <summary>
    /// Edits strings in the UI
    /// </summary>
    class UIFixes: IAssetEditor
    {
        private IModHelper modHelper;
        public UIFixes(IModHelper helper)
        {
            modHelper = helper;
        }
        //Allow asset to be editted if name matches
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Strings\\UI"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Edit asset
        public void Edit<T>(IAssetData asset)
        {
           
            var UIeditor = asset.AsDictionary<string, string>().Data;

            UIeditor["ItemList_ItemsLost"] = "Items recovered:";
        }
    }
}
