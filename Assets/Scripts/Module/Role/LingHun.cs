using Framework.Message;
using network;
using System;
using System.Collections.Generic;

namespace AGrail
{
    public class LingHun : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.LingHun;
            }
        }

        public override string RoleName
        {
            get
            {
                return "灵魂术士";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.幻;
            }
        }

        public override bool HasYellow
        {
            get
            {
                return true;
            }
        }

        public override bool HasBlue
        {
            get
            {
                return true;
            }
        }

        public LingHun()
        {
            for (uint i = 2201; i <= 2208; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case (uint)SkillID.灵魂震爆:
                case (uint)SkillID.灵魂赐予:
                    return card.HasSkill(uiState);
                case (uint)SkillID.灵魂召还:
                    return card.Type == Card.CardType.magic;
                case (uint)SkillID.灵魂镜像:
                    return true;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case (uint)SkillID.灵魂震爆:
                case (uint)SkillID.灵魂赐予:
                case (uint)SkillID.灵魂镜像:
                    return true;
                case (uint)SkillID.灵魂链接:
                    return player.team == BattleData.Instance.MainPlayer.team && player.id != BattleData.Instance.PlayerID;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 10:
                case 11:
                case (uint)SkillID.灵魂震爆:
                case (uint)SkillID.灵魂赐予:
                case (uint)SkillID.灵魂召还:
                case (uint)SkillID.灵魂镜像:
                    if (skill.SkillID == (uint)SkillID.灵魂震爆 && BattleData.Instance.MainPlayer.yellow_token >= 3)
                        return true;
                    if (skill.SkillID == (uint)SkillID.灵魂赐予 && BattleData.Instance.MainPlayer.blue_token >= 3)
                        return true;
                    if (skill.SkillID == (uint)SkillID.灵魂召还)                        
                        return true;
                    if (skill.SkillID == (uint)SkillID.灵魂镜像 && BattleData.Instance.MainPlayer.yellow_token >= 2)
                        return true;
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.灵魂震爆:
                case (uint)SkillID.灵魂赐予:
                    return 1;
                case (uint)SkillID.灵魂召还:
                    return BattleData.Instance.MainPlayer.max_hand;
                case (uint)SkillID.灵魂镜像:
                    return Math.Min(BattleData.Instance.MainPlayer.hand_count, 3);
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.灵魂震爆:
                case (uint)SkillID.灵魂赐予:
                case (uint)SkillID.灵魂镜像:
                case (uint)SkillID.灵魂链接:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.灵魂震爆:
                case (uint)SkillID.灵魂赐予:
                    return cardIDs.Count == 1 && playerIDs.Count == 1;
                case (uint)SkillID.灵魂召还:
                    return cardIDs.Count > 0;
                case (uint)SkillID.灵魂镜像:
                    return cardIDs.Count == Math.Min(BattleData.Instance.MainPlayer.hand_count, 3) && playerIDs.Count == 1;
                case (uint)SkillID.灵魂增幅:
                case (uint)SkillID.灵魂转换:
                case (uint)SkillID.灵魂链接:
                case (uint)SkillID.灵魂链接响应:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.灵魂震爆:
                case (uint)SkillID.灵魂赐予:
                case (uint)SkillID.灵魂召还:
                case (uint)SkillID.灵魂镜像:
                case (uint)SkillID.灵魂增幅:
                case (uint)SkillID.灵魂转换:
                case (uint)SkillID.灵魂链接:
                case (uint)SkillID.灵魂链接响应:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            List<List<uint>> selectList = new List<List<uint>>();
            List<string> mList = new List<string>();
            switch (state)
            {
                case (uint)SkillID.灵魂震爆:
                case (uint)SkillID.灵魂赐予:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, 
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择目标玩家以及独有技卡牌", Skills[state].SkillName));
                    return;
                case (uint)SkillID.灵魂召还:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            null, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择法术牌", Skills[state].SkillName));
                    return;
                case (uint)SkillID.灵魂镜像:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择要舍弃的牌以及目标角色", Skills[state].SkillName));
                    return;
                case (uint)SkillID.灵魂增幅:                
                    OKAction = () =>
                    {
                        IsStart = true;
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {                        
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };                    
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("是否发动{0}", Skills[state].SkillName));
                    return;
                case (uint)SkillID.灵魂转换:
                    OKAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 2 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                    selectList.Clear();
                    mList.Clear();
                    if (BattleData.Instance.MainPlayer.yellow_token > 0)
                    {
                        selectList.Add(new List<uint>() { 0 });
                        mList.Add("将一点黄魂转换为蓝魂");
                    }
                    if (BattleData.Instance.MainPlayer.blue_token > 0)
                    {
                        selectList.Add(new List<uint>() { 1 });
                        mList.Add("将一点蓝魂转换为黄魂");
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "选择技能", selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("是否发动{0}", Skills[state].SkillName));
                    return;
                case (uint)SkillID.灵魂链接:
                    OKAction = () =>
                    {
                        IsStart = true;
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, 
                            BattleData.Instance.Agent.SelectPlayers, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("是否发动{0}", Skills[state].SkillName));
                    return;
                case (uint)SkillID.灵魂链接响应:
                    OKAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                    selectList.Clear();
                    mList.Clear();
                    for(uint i = Math.Min(BattleData.Instance.MainPlayer.blue_token, BattleData.Instance.Agent.Cmd.args[0]); i > 0; i--)
                    {
                        selectList.Add(new List<uint>() { i });
                        mList.Add("点伤害");
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "选择转移的伤害", selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择要转移的伤害", Skill.GetSkill(state).SkillName));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

        private enum SkillID
        {
            灵魂震爆 = 2201,
            灵魂赐予,
            灵魂增幅,
            灵魂吞噬,
            灵魂召还,
            灵魂转换,
            灵魂镜像,
            灵魂链接,
            灵魂链接响应
        }
    }
}
