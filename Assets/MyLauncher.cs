using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class MyLauncher : MonoBehaviourPunCallbacks
{
    public InputField m_inputField_Nickname; // �г��� �Է¹޴� ��.
    public Dropdown m_dropdown_RoomMaxPlayers; // �ִ� �ο� �� ����� ����.
    public Dropdown m_dropdown_MaxTime; // ���� �ð��� �� �ʷ� ���� ����.
    public GameObject m_panel_Loading; // �ε� UI.
    public Text m_text_CurrentPlayerCount; // �ε� UI �߿��� ���� �ο� ���� ��Ÿ��.

    void Awake()
    {
        // ������ Ŭ���̾�Ʈ�� PhotonNetwork.LoadLevel()�� ȣ���� �� �ְ�, ��� ����� �÷��̾�� �ڵ������� ������ ������ �ε��Ѵ�.
        PhotonNetwork.AutomaticallySyncScene = true;

        //m_panel_Loading.SetActive(false);
    }

    void Start()
    {
        print("���� ���� �õ�.");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinRandomOrCreateRoom()
    {
        string nick = m_inputField_Nickname.text;

        print($"{nick} ���� ��Ī ����.");
        PhotonNetwork.LocalPlayer.NickName = nick; // ���� �÷��̾� �г��� �����ϱ�.

        // UI���� �� ������.
        byte maxPlayers = byte.Parse(m_dropdown_RoomMaxPlayers.options[m_dropdown_RoomMaxPlayers.value].text); // ��Ӵٿ�� �� ������.
        int maxTime = int.Parse(m_dropdown_MaxTime.options[m_dropdown_MaxTime.value].text);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers; // �ο� ����.
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "maxTime", maxTime } }; // ���� �ð� ����.
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "maxTime" }; // ���⿡ Ű ���� ����ؾ�, ���͸��� �����ϴ�.

        // �� ������ �õ��ϰ�, �����ϸ� �����ؼ� ������.
        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: new ExitGames.Client.Photon.Hashtable() { { "maxTime", maxTime } }, expectedMaxPlayers: maxPlayers, // ������ ���� ����.
            roomOptions: roomOptions // ������ ���� ����.
        );
    }

    public void CancelMatching()
    {
        print("��Ī ���.");
        m_panel_Loading.SetActive(false);

        print("�� ����.");
        PhotonNetwork.LeaveRoom();
    }

    private void UpdatePlayerCounts()
    {
        m_text_CurrentPlayerCount.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    #region ���� �ݹ� �Լ�

    
    public override void OnJoinedRoom()
    {
        print("�� ���� �Ϸ�.");

        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}�� �ο��� {PhotonNetwork.CurrentRoom.MaxPlayers} ��Ī ��ٸ��� ��.");
        UpdatePlayerCounts();

        m_panel_Loading.SetActive(true);
    }


    //�÷��̾ �뿡 ���Դ��� üũ���ش�.
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log($"�÷��̾� {newPlayer.NickName} �� ����.");
        UpdatePlayerCounts();

        if (PhotonNetwork.IsMasterClient)
        {
            // ��ǥ �ο� �� ä������, �� �̵��� �Ѵ�. ������ ������ Ŭ���̾�Ʈ��.
            // PhotonNetwork.AutomaticallySyncScene = true; �� ������ �濡 ������ �ο��� ��� �̵���.

            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.LoadLevel("Game");
            }
        }
    }


    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"�÷��̾� {otherPlayer.NickName} �� ����.");
        UpdatePlayerCounts();
    }


    #endregion
}
