using UnityEngine;
using UnityEngine.UI;

using System.Text;

using Adrenak.GoGo;

namespace Adrenak.GoGo.Examples {
    public class InputPrinter : MonoBehaviour {
        [SerializeField] Text display;

        StringBuilder builder = new StringBuilder();

        void Update() {
            builder.Clear();

            var message = builder
                .Append(GoInput.GetAngles()).Append("\n")
                .Append(GoInput.Orientation).Append("\n")
                .Append(GoInput.GetDirection()).Append("\n")
                .ToString();

            display.text = message;
        }
    }
}
