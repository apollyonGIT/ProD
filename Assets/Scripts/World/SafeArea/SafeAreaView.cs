using Addrs;
using Foundations;
using UnityEngine;
using UnityEngine.UI;
using World.Business;
using World.Characters;

namespace World.SafeArea
{
    public class SafeAreaView : MonoBehaviour
    {
        public Button character_button;

        public void Init()
        {
            Mission.instance.try_get_mgr("BusinessMgr", out BusinessMgr bmgr);
            var character_business = new Business.Business();
            bmgr.AddBusiness(character_business);

            Addressable_Utility.try_load_asset<CharacterBusinessPanel>("CharacterBusinessPanel", out var cbp);
            var cbp_ins = Instantiate(cbp, transform, false);
            cbp_ins.gameObject.SetActive(false);
            character_business.add_view(cbp_ins.character_business_view);

            Mission.instance.try_get_mgr(Commons.Config.CharacterMgr_Name, out CharacterMgr cmgr);
            cmgr.add_view(cbp_ins.character_mgr_view);

            character_business.Init(2000);

            character_button.onClick.AddListener(() =>
            {
                cbp_ins.gameObject.SetActive(true);
            });
        }
    }
}
