﻿namespace Framework.Message
{
    public enum MessageType
    {
        // Const Message Type
        Null = 0,
        OnConnect,
        OnDisconnect,
        OnReconnect,
        OnUICreate,
        OnUIDestroy,
        OnUIShow,
        OnUIHide,
        OnUIPause,
        OnUIResume,

        // Protobuf Message Type
        REGISTERREQUEST,
        REGISTERRESPONSE,
        LOGINREQUEST,
        LOGINRESPONSE,
        LOGOUTREQUEST,
        LOGOUTRESPONSE,
        ROOMLISTREQUEST,
        ROOMLISTRESPONSE,
        CREATEROOMREQUEST,
        ENTERROOMREQUEST,
        LEAVEROOMREQUEST,
        JOINTEAMREQUEST,
        READYFORGAMEREQUEST,
        SINGLEPLAYERINFO,
        GAMEINFO,
        TALK,
        GOSSIP,
        ERROR,
        ROLEREQUEST,
        PICKBAN,
        ACTION,
        RESPOND,
        COMMANDREQUEST,
        ERRORINPUT,
        HITMSG,
        TURNBEGIN,
        CARDMSG,
        HURTMSG,
        SKILLMSG,

        // Custom Message Type
        LoginState,
        RoomList,
        InputBox,
        EnterRoom,
        RoomIDChange,
        GameStart,
        ChooseRole,
        MoraleChange,
        ShowCardChange,
        GemChange,
        CrystalChange,
        GrailChange,
        SendHint,
        Win,
        Lose,
        PlayerLeave,
        PlayerHeroName,
        PlayerRoleChange,
        PlayerTeamChange,
        PlayerHandChange,
        PlayerHealChange,
        PlayerEnergeChange,
        PlayerTokenChange,
        PlayerKneltChange,
        PlayerNickName,
        PlayerIsReady,
        PlayerBasicAndExCardChange,
        PlayerActionChange,
        LogChange,
        ChatChange,
        AgentSelectCard,
        AgentSelectPlayer,
        AgentSelectSkill,
        AgentSelectArgs,
        AgentStateChange,
        AgentUIStateChange,
        AgentHandChange,
        AgentUpdate,
        ShowNewArgsUI,
        CloseNewArgsUI,
        PlayBGM,
    }
}
