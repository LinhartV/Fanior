
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class SlingshotWeapon : Weapon
    {
        public SlingshotWeapon(bool autoFire, double reloadTime, int shotSpeed, double damage, double lifeSpan, string name, string imageName) : base(autoFire, reloadTime, shotSpeed, damage, lifeSpan, name, imageName)
        {

        }

        protected override void CreateShot(Gvars gvars)
        {
            try
            {
                var c = gvars.Items[CharacterId] as Character;
                c.AddAction(gvars, new ItemAction("slingshot", 0, ItemAction.ExecutionType.OnlyFirstTime), 0, ActionHandler.RewriteEnum.AddNew);
                c.AddAction(gvars, new ItemAction("slingshot", ToolsGame.random.Next(2, 10), ItemAction.ExecutionType.OnlyFirstTime), 0, ActionHandler.RewriteEnum.AddNew);
                c.AddAction(gvars, new ItemAction("slingshot", ToolsGame.random.Next(10, 20), ItemAction.ExecutionType.OnlyFirstTime), 0, ActionHandler.RewriteEnum.AddNew);

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
