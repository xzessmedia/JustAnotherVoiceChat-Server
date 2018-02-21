﻿/*
 * File: VoiceScript.Events.cs
 * Date: 15.2.2018,
 *
 * MIT License
 *
 * Copyright (c) 2018 JustAnotherVoiceChat
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared.Gta.Tasks;
using JustAnotherVoiceChat.Server.GTMP.Interfaces;

namespace JustAnotherVoiceChat.Server.GTMP.Resource
{
    public partial class VoiceScript
    {

        private void AttachToVoiceServerEvents()
        {
            _voiceServer.OnServerStarted += () =>
            {
                var config = _voiceServer.Configuration;
                API.consoleOutput(LogCat.Info, $"GtmpVoiceServer started, listening on {config.Hostname}:{config.Port}");
            };
            
            _voiceServer.OnServerStopping += () =>
            {
                API.consoleOutput(LogCat.Info, "GtmpVoiceServer stopping!");
            };

            _voiceServer.OnClientPrepared += OnClientPrepared;
            
            _voiceServer.OnClientConnected += OnClientConnected;
            _voiceServer.OnClientDisconnected += OnHandshakeShouldResend;

            _voiceServer.OnClientTalkingChanged += OnPlayerTalkingChanged;
        }

        private void OnClientConnected(IGtmpVoiceClient client)
        {
            client.Player.triggerEvent("VOICE_SET_HANDSHAKE", false);
            client.SetNickname(client.Player.name);
        }

        private void OnClientPrepared(IGtmpVoiceClient client)
        {
            _voiceServer.NativeWrapper.SetClientVoiceRange(client, 15);
            
            OnHandshakeShouldResend(client);
        }

        private void OnHandshakeShouldResend(IGtmpVoiceClient client)
        {
            client.Player.triggerEvent("VOICE_SET_HANDSHAKE", true, client.HandshakeUrl);
        }
        
        private void OnPlayerTalkingChanged(IGtmpVoiceClient speakingClient, bool newStatus)
        {
            if (newStatus)
            {
                API.playPlayerAnimation(speakingClient.Player, (int)(AnimationFlag.Loop | AnimationFlag.AllowRotation), "mp_facial", "mic_chatter");
            }
            else
            {
                API.stopPlayerAnimation(speakingClient.Player);
            }
        }
        
    }
}