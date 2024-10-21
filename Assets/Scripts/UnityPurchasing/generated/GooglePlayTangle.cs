// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("GOfGHcD/PqL3gV3A9UJNqYrnAICBkKRB7fppk1PBRPQVPaEmGboVhbY+2wm0RzjBefM7tiLFQU3KSIQ9qm2FmDlPPFcCFkR9hiriF8MTzctS0BcuHJ9TtbiNvcXAItAuCdDgmifrTt5+nOT2wrow+NIy8OF3bQnNwgJELrE4aVrxDEwIjBnX+NtHzzm6hlYAWLELZJSmeYX9RY4b63ryldtp6snb5u3iwW2jbRzm6urq7uvoRNqAgO/Glf4q1qsxPYjRNrSZ32+S5jsoDKyyy7/fpoClWUj6CYTZg2nq5Ovbaerh6Wnq6usuAzpCzNHAOgPFzXQkeyK4QVq7dCyQjbItcvYRtDkG38Tvig5sm+C6aTUGbW2Vx0h9xxTrFQLpLOno6uvq");
        private static int[] order = new int[] { 13,2,12,3,5,9,8,8,13,9,13,12,13,13,14 };
        private static int key = 235;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
