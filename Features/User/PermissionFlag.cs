using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.User
{
    internal class PermissionFlag
    {
        public static readonly byte BUBBLEFARM = 1, ADMIN = 2, MOD = 4, CLOUD = 8, PATRON = 16, PERMABAN = 32, DROPBAN = 64, PATRONIZER = 128;

        // Flag schema:
        // M... Mod - A... Admin - F... Farming - T... Full Typo Cloud Access
        // F =  P T M A F
        // 0 =  0 0 0 0 0
        // 1 =  0 0 0 0 1
        // 2 =  0 0 0 1 0
        // 4 =  0 0 1 0 0
        // 8 =  0 1 0 0 0
        // 16 =  1 0 0 0 0
        // ... etc

        public bool BotAdmin { get; private set; }
        public bool BubbleFarming { get; private set; }
        public bool Moderator { get; private set; }
        public bool CloudUnlimited { get; private set; }
        public bool Patron { get; private set; }
        public bool Permanban { get; private set; }
        public bool Dropban { get; private set; }
        public bool Patronizer { get; private set; }

        public PermissionFlag(byte flag)
        {
            BitArray flags = new BitArray(new byte[] { flag });
            BubbleFarming = flags[0];
            BotAdmin = flags[1];
            Moderator = flags[2];
            CloudUnlimited = flags[3];
            Patron = flags[4];
            Permanban = flags[5];
            Dropban = flags[6];
            Patronizer = flags[7];
        }

        public int CalculateFlag()
        {
            return (BubbleFarming ? BUBBLEFARM : 0)
                + (BotAdmin ? ADMIN : 0)
                + (Moderator ? MOD : 0)
                + (CloudUnlimited ? CLOUD : 0)
                + (Patron ? PATRON : 0)
                + (Permanban ? PERMABAN : 0)
                + (Dropban ? DROPBAN : 0)
                + (Patronizer ? PATRONIZER : 0);
        }

        public bool CheckForPermissionByte(byte permission)
        {
            BitArray presentFlags = new BitArray(new byte[] { (byte) CalculateFlag() });
            BitArray checkFlags = new BitArray(new byte[] { permission });

            bool allMatch = true;
            for (int flagIndex = 0; flagIndex < checkFlags.Length; flagIndex++)
            {
                if (checkFlags[flagIndex] && (presentFlags.Length - 1 < flagIndex || !presentFlags[flagIndex])) allMatch = false;
            }
            return allMatch;
        }
    }
}
