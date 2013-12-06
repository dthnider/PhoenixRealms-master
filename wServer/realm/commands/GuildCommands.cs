﻿#region

using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.realm.commands
{
    internal class GuildChatCommand : ICommand
    {
        private ClientProcessor psr;

        public string Command
        {
            get { return "g"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            if (player.Guild != "")
            {
                try
                {
                    var saytext = string.Join(" ", args);

                    foreach (var w in RealmManager.Worlds)
                    {
                        var world = w.Value;
                        if (w.Key != 0) // 0 is limbo??
                        {
                            foreach (var i in world.Players)
                            {
                                if (i.Value.Guild == player.Guild)
                                {
                                    var tp = new TextPacket
                                    {
                                        BubbleTime = 10,
                                        Stars = player.Stars,
                                        Name = player.ResolveGuildChatName(),
                                        Recipient = "*Guild*",
                                        Text = " " + saytext
                                    };
                                    if (world.Id == player.Owner.Id) tp.ObjectId = player.Id;
                                    i.Value.Client.SendPacket(tp);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    player.SendInfo("Cannot guild chat!");
                }
            }
            else
                player.SendInfo("You need to be in a guild to use guild chat!");
        }
    }

    internal class InviteCommand : ICommand
    {
        public string Command
        {
            get { return "invite"; }
        }

        public int RequiredRank
        {
            get { return 0; }
        }

        public void Execute(Player player, string[] args)
        {
            if (player.GuildRank >= 20)
            {
                foreach (var i in RealmManager.Worlds)
                {
                    if (i.Key != 0)
                    {
                        foreach (var e in i.Value.Players)
                        {
                            if (e.Value.Client.Account.Name.ToLower() == args[0].ToLower())
                            {
                                if (e.Value.Client.Account.Guild.Name == "")
                                {
                                    e.Value.Client.SendPacket(new InvitedToGuildPacket
                                    {
                                        Name = player.Client.Account.Name,
                                        Guild = player.Client.Account.Guild.Name
                                    });
                                }
                                else
                                {
                                    player.Client.SendPacket(new TextPacket
                                    {
                                        BubbleTime = 0,
                                        Stars = -1,
                                        Name = "*Error*",
                                        Text = e.Value.Client.Account.Name + " is already in a guild!"
                                    });
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                {
                    player.SendInfo("Members and initiates cannot invite!");
                }
            }
        }
    }
}