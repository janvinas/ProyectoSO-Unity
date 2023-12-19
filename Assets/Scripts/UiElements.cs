using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiElements : MonoBehaviour
{
    public Image mainPanel;
    public GameObject mainPanelServerAddress;
    public Button mainPanelConnectButton;
    public Button mainPanelLoginButton;
    public Button mainPanelRegisterButton;
    public TextMeshProUGUI mainPanelMessageBox;
    public Image mainPanelUserInfo;
    public TextMeshProUGUI mainPanelUsername;
    public TextMeshProUGUI mainPanelExperience;
    public Image mainPanelProfilePicture;
    public Image mainPanelListaConectados;
    public TextMeshProUGUI mainPanelNumeroConectados;
    public Button mainPanelInvitarJugadores;
    public GameObject mainPanelChat;
    public GameObject mainPanelChatTitle;
    public GameObject mainPanelChatInput;

    public Image notificionInvitacionPanel;

    public Image loginPanel;
    public Image registerPanel;

    public Color accentColor = new Color(0x14 / 255f, 0x18 / 255f, 0x49 / 255f);
    public Color bgColor = new Color(0x32 / 255f, 0xca / 255f, 0xe7 / 255f);

    public TMP_FontAsset font;

}
