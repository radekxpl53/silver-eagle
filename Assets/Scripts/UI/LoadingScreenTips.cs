using UnityEngine;
using TMPro;

public class LoadingScreenTips : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tipText;

    private static readonly string[] Tips = new string[]
    {
        "TIP: Space mining in Rubieze is illegal without a proper guild license, but yields are 200% higher.",
        "TIP: Keep an eye on your thermal hints during mining. Overheating can blow your cargo deck.",
        "TIP: Cermandia patrols will scan for illegal scrap. Keep your weapon systems powered down when entering Ariandia zone.",
        "TIP: Fuel drain increases by 50% when cargo occupancy is above 80%. Drop cheap resources if chased.",
        "TIP: The heavy kinetic railgun has slow reload times but pierces pirate armor with ease.",
        "TIP: Fuel ratio vs Cargo capacity can be reconfigured before departing at the Navigation Table.",
        "TIP: Erad'os sector has a 45% trade tax. Plan your sell routes accordingly.",
        "TIP: Press E near the Nav Table in the cockpit to select your next jump destination.",
        "TIP: Upgrades purchased at the station are automatically loaded onto the ship status monitor.",
        "TIP: Combat speed is severely impacted when dragging maximum raw minerals.",
        "TIP: Sector A6 (Keimos) is monitored closely by Cermandia patrol fleets.",
        "TIP: Always pay down your debt. High interest rate can eat your profits overnight.",
        "TIP: Pirate Cove in sector C5 is a hotspot for combat. Arm shields before entry.",
        "TIP: Outposts in Uranus Orbit (E2) sell specialized heat-resistant mining lasers.",
        "TIP: Press WASD to fly the ship. Keep inertia in mind when making high-speed turns.",
        "TIP: In extreme threat levels, emergency thrusters will activate even if main energy is depleted.",
        "TIP: Scrap and silicates can be recycled at any local Cermandia depot.",
        "TIP: Energy regeneration depends on the ship's reactor core upgrade levels.",
        "TIP: Mrainesden (F5) is restricted military space. Violators will be scanned and fired upon.",
        "TIP: Use E to interact with resource-rich asteroids in your radar vicinity."
    };

    private void Start()
    {
        if (tipText != null)
        {
            tipText.text = Tips[Random.Range(0, Tips.Length)];
        }
    }
}
