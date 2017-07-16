using System.Collections.Generic;
using network;
using System;
using Framework.Message;

namespace AGrail
{
    public class ShenGuan : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.ShenGuan;
            }
        }

        public override string RoleName
        {
            get
            {
                return "神官";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.圣;
            }
        }

        public override uint MaxHealCount
        {
            get
            {
                return 6;
            }
        }

        public ShenGuan()
        {
            for (uint i = 1501; i <= 1506; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case (uint)SKILLID.神圣祈福:
                    return card.Type == Card.CardType.magic;
                case (uint)SKILLID.水之神力:
                    if (additionalState == 15031)
                        return true;
                    return card.Element == Card.CardElement.water;
                case (uint)SKILLID.神圣领域:
                    if (additionalState != 15061 && additionalState != 15062)
                        return true;
                    return false;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case (uint)SKILLID.水之神力:
                case (uint)SKILLID.神圣契约:
                    return player.team == BattleData.Instance.MainPlayer.team && BattleData.Instance.PlayerID != player.id;
                case (uint)SKILLID.神圣领域:
                    if(additionalState == 15061)
                        return true;
                    if(additionalState == 15062)
                        return player.team == BattleData.Instance.MainPlayer.team;
                    return false;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 10:
                case 11:                    
                case (uint)SKILLID.神圣祈福:
                case (uint)SKILLID.水之神力:
                case (uint)SKILLID.神圣领域:
                    if (skill.SkillID == 1506 && BattleData.Instance.MainPlayer.crystal + BattleData.Instance.MainPlayer.gem > 0)
                        return true;
                    if (skill.SkillID >= 1502 && skill.SkillID <= 1503)
                        return true;
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SKILLID.神圣祈福:                
                    return 2;
                case (uint)SKILLID.水之神力:
                case (uint)SKILLID.水之神力给牌:
                    return 1;
                case (uint)SKILLID.神圣领域:
                    if (additionalState == 15061 || additionalState == 15062)
                        return 0;
                    return 2;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SKILLID.水之神力:                    
                    return 1;                
                case (uint)SKILLID.神圣契约:
                    return 1;
                case (uint)SKILLID.神圣领域:
                    if (additionalState == 15061 || additionalState == 15062)
                        return 1;
                    return 0;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SKILLID.神圣启示:
                    return true;
                case (uint)SKILLID.神圣祈福:
                    return cardIDs.Count == 2;
                case (uint)SKILLID.水之神力:
                    return cardIDs.Count == 1 && playerIDs.Count == 1 && BattleData.Instance.MainPlayer.hand_count > 1;
                case (uint)SKILLID.水之神力给牌:
                    return cardIDs.Count == 1;
                case (uint)SKILLID.神圣契约:
                    return playerIDs.Count == 1;
                case (uint)SKILLID.神圣领域:
                    if (additionalState == 15061 || additionalState == 15062)
                        return playerIDs.Count == 1;
                    return cardIDs.Count == Math.Min(BattleData.Instance.MainPlayer.hand_count, 2);
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SKILLID.神圣启示:
                case (uint)SKILLID.神圣祈福:
                case (uint)SKILLID.水之神力:
                case (uint)SKILLID.神圣契约:
                case (uint)SKILLID.神圣领域:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            List<List<uint>> selectList = new List<List<uint>>();
            if (state != (uint)SKILLID.水之神力 && state != (uint)SKILLID.神圣领域)
                additionalState = 0;
            switch (state)
            {
                case (uint)SKILLID.神圣启示:
                    OKAction = () =>
                    {
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
                case (uint)SKILLID.神圣祈福:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, null, 
                            BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 选择两张法术", Skills[state].SkillName));
                    return;
                case (uint)SKILLID.水之神力:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, 
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 选择一张水系牌以及目标队友", Skills[state].SkillName));                    
                    return;
                case (uint)SKILLID.水之神力给牌:
                    OKAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 选择要给予的牌", Skills[state].SkillName));
                    return;
                case (uint)SKILLID.神圣契约:
                    OKAction = () =>
                    {
                        IsStart = true;
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers, 
                            null, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => 
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                    selectList.Clear();
                    for (uint i = Math.Min(4, BattleData.Instance.MainPlayer.heal_count); i >= 1; i--)
                        selectList.Add(new List<uint>() { i });
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "Heal", selectList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 选择想要给予治疗的目标队友以及治疗数量", Skills[state].SkillName));
                    return;
                case (uint)SKILLID.神圣领域:
                    OKAction = () =>
                    {
                        if(additionalState == 15061)
                        {
                            sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state, new List<uint>() { 1 });
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        }
                        else if(additionalState == 15062)
                        {
                            sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state, new List<uint>() { 2 });
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        }
                        else
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                            if (BattleData.Instance.Agent.SelectArgs[0] == 1)
                                additionalState = 15061;
                            else
                                additionalState = 15062;
                            BattleData.Instance.Agent.RemoveSelectPlayer(0);
                        }
                    };
                    CancelAction = () => 
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };                    
                    if (additionalState == 15061)
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 选择想要造成伤害的目标角色", Skills[state].SkillName));
                    else if(additionalState == 15062)
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 选择想要给予治疗的目标队友", Skills[state].SkillName));
                    else if(msg == UIStateMsg.ClickSkill)
                    {
                        selectList.Clear();
                        var mList = new List<string>();
                        if (BattleData.Instance.MainPlayer.heal_count > 0)
                        {
                            selectList.Add(new List<uint>() { 1 });
                            mList.Add(" （移除你的1[治疗]）对目标角色造成2点法术伤害");
                        }
                        selectList.Add(new List<uint>() { 2 });
                        mList.Add(" 你+2[治疗]，目标队友+1[治疗]");
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "选择技能", selectList, mList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 选择手牌以及技能效果", Skills[state].SkillName));
                    }                        
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

        private enum SKILLID
        {
            神圣启示 = 1501,
            神圣祈福,
            水之神力,
            圣使守护,
            神圣契约,
            神圣领域,
            水之神力给牌 = 1531
        }

    }
}
