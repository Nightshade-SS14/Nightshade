using System.Text;
using Content.Trauma.Common.Language.Systems;

namespace Content.Trauma.Common.Language;

// Adapted from Starlight's RandomObfuscation implementation.
// See LICENSE-Starlight.TXT.
public sealed partial class RandomObfuscation : ObfuscationMethod
{
    public override void Obfuscate(
        StringBuilder builder,
        string message,
        CommonLanguageSystem context,
        float ratio = 1.0f)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        message = message.ToUpper();

        for (var i = 0; i < chars.Length; i++)
        {
            message = message.Replace(
                chars[i],
                chars[context.PseudoRandomNumber(
                    message.GetHashCode() + i,
                    0,
                    chars.Length - 1)]);
        }

        builder.Append(message);
    }
}
