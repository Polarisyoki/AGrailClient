using System.Collections.Generic;
using network;
using Framework.Message;
using System.Linq;

namespace AGrail
{
    public class HongLian : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.HongLian;
            }
        }

        public override string RoleName
        {
            get
            {
                return "红莲骑士";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.血;
            }
        }

        public override bool HasYellow
        {
            get
            {
                return true;
            }
        }

        public override string Knelt
        {
            get
            {
                return "ReXueFeiTeng";
            }
        }

        public override uint MaxHealCount
        {
            get
            {
                return 4;
            }
        }

        public HongLian()
        {
            for (uint i = 2801; i <= 2803; i++)
                Skills.Add(i, Skill.GetSkill(i));
            for (uint i = 2806; i <= 2809; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case (uint)SkillID.腥红十字:
                    return card.Type == Card.CardType.magic;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case (uint)SkillID.腥红十字:
                    return true;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 10:
                case 11:
                case (uint)SkillID.腥红十字:
                    if (skill.SkillID == (uint)SkillID.腥红十字 &&
                        BattleData.Instance.MainPlayer.gem + BattleData.Instance.MainPlayer.crystal > 0)
                        return true;
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.腥红十字:
                    return 2;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.腥红十字:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.腥红圣约:
                case (uint)SkillID.血腥祷言:
                case (uint)SkillID.杀戮盛宴:
                case (uint)SkillID.戒骄戒躁:
                    return true;
                case (uint)SkillID.腥红十字:
                    return cardIDs.Count == 2 && playerIDs.Count == 1;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.腥红圣约:
                case (uint)SkillID.血腥祷言:
                case (uint)SkillID.杀戮盛宴:
                case (uint)SkillID.戒骄戒躁:
                case (uint)SkillID.腥红十字:
                    return true;                
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        private List<uint> selectPlayers = new List<uint>();
        private List<uint> selectHealCnt = new List<uint>();
        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case (uint)SkillID.腥红圣约:
                case (uint)SkillID.杀戮盛宴:
                case (uint)SkillID.戒骄戒躁:
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
                case (uint)SkillID.血腥祷言:
                    OKAction = () =>
                    {                        
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        if (additionalState == 0)
                        {
                            selectPlayers.Clear();
                            selectHealCnt.Clear();
                            selectPlayers.Add(allies[0].id);
                            selectHealCnt.Add(BattleData.Instance.Agent.SelectArgs[0]);
                            if(allies.Count == 1)
                            {
                                IsStart = true;
                                sendReponseMsg(state, BattleData.Instance.MainPlayer.id, 
                                    selectPlayers, null, selectHealCnt);
                            }
                            else
                            {
                                additionalState = 28031;
                                BattleData.Instance.Agent.RemoveSelectPlayer(0);
                            }                            
                        }
                        else
                        {
                            IsStart = true;
                            if(selectHealCnt[0] == 0)
                            {
                                selectHealCnt.Clear();
                                selectPlayers.Clear();
                            }
                            selectPlayers.Add(allies[1].id);
                            selectHealCnt.Add(BattleData.Instance.Agent.SelectArgs[0]);                            
                            sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                                    selectPlayers, null, selectHealCnt);
                            additionalState = 0;
                        }
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        if(additionalState == 0)
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                            sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        }
                        else
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                            additionalState = 0;
                            BattleData.Instance.Agent.RemoveSelectPlayer(0);
                        }
                    };
                    if(additionalState == 0)
                    {
                        List<List<uint>> selectList = new List<List<uint>>();
                        for (uint i = 0; i <= BattleData.Instance.MainPlayer.heal_count; i++)
                            selectList.Add(new List<uint>() { i });
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "Heal", selectList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 选择给予{1}的治疗数", Skills[state].SkillName, allies[0].nickname));
                    }
                    else
                    {
                        List<List<uint>> selectList = new List<List<uint>>();
                        for (uint i = 0; i <= BattleData.Instance.MainPlayer.heal_count - selectHealCnt[0]; i++)
                            selectList.Add(new List<uint>() { i });
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "Heal", selectList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 选择给予{1}的治疗数", Skills[state].SkillName, allies[1].nickname));
                    }
                    return;
                case (uint)SkillID.腥红十字:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 选择两张法术牌及一个目标玩家", Skills[state].SkillName));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

        private List<SinglePlayerInfo> allies
        {
            get
            {
                return BattleData.Instance.PlayerInfos.Where((s) => { return s.team == BattleData.Instance.MainPlayer.team && s.id != BattleData.Instance.MainPlayer.id; }).ToList();
            }            
        }

        private enum SkillID
        {
            腥红圣约 = 2801,
            腥红信仰,
            血腥祷言,
            杀戮盛宴 = 2806,
            热血沸腾,
            戒骄戒躁,
            腥红十字
        }
    }
}


