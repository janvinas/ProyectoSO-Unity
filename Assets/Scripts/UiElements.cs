using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiElements : MonoBehaviour
{
    public Image mainPanel;
    public GameObject mainPanelServerAddress;
    public Button mainPanelConnectButton;
    public Image mainPanelConnectionIndicator;
    public Button mainPanelLoginButton;
    public Button mainPanelRegisterButton;
    public TextMeshProUGUI mainPanelMessageBox;
    public Image mainPanelUserInfo;
    public TextMeshProUGUI mainPanelUsername;
    public TextMeshProUGUI mainPanelExperience;
    public Image mainPanelListaConectados;
    public TextMeshProUGUI mainPanelNumeroConectados;
    public Button mainPanelInvitarJugadores;
    public GameObject mainPanelIniciarPartida;
    public GameObject mainPanelChat;
    public GameObject mainPanelChatInput;
    public Button mainPanelDesconectarButton;
    public Button mainPanelEliminarButton;
    public Button mainPanelConsultasButton;
    public Button mainPanelPracticarButton;

    public Image notificionInvitacionPanel;

    public Image loginPanel;
    public Image registerPanel;

    public Color accentColor = new Color(0x14 / 255f, 0x18 / 255f, 0x49 / 255f);
    public Color bgColor = new Color(0x32 / 255f, 0xca / 255f, 0xe7 / 255f);

    public TMP_FontAsset font;

}
