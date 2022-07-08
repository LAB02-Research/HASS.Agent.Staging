<a href="https://github.com/LAB02-Research/HASS.Agent/">
    <img src="https://raw.githubusercontent.com/LAB02-Research/HASS.Agent/main/images/logo_128.png" alt="HASS.Agent logo" title="HASS.Agent" align="right" height="128" /></a>

# HASS.Agent Staging Project

**Do not use this repo if you just want to manually build and use HASS.Agent. Please see [these instructions](https://hassagent.readthedocs.io/en/latest/installation/#3-build-from-scratch) on how to do that.**

<br/><br/>
This project contains the latest (beta) code of all three parts of the HASS.Agent platform:
<br/><br/>

| Project | Description |
|---|---|
| HASS.Agent | Main client, containing the UI, runs in userspace, by default without elevation |
| HASS.Agent.Satellite.Service | Windows client, runs under SYSTEM account |
| HASS.Agent.Shared | Library, published as a nuget, contains all commands, sensors, shared functions and enums |

<br/>

It's purpose is to more easily develop beta releases, as the shared library will be instantly compiled and referenced.

Note: make sure any regular instance of HASS.Agent is shut down. If you're just planning to tinker with HASS.Agent, you can leave the regular service active, otherwise turn it off as well as the named pipe will be in use. Start the service with elevated privileges.

Note: it's best to have `enable extended logging` enabled, which will also reflect on the satellite service (as long as it's started in console mode instead of service mode). But that'll also generate false positives, so primarily focus on the issue at hand.

----

The base of all tickets is HASS.Agent's YouTrack page:

[HASS.Agent YouTrack Dashboard](https://lab02research.youtrack.cloud)

Let me know that you're working on a ticket (you can use the discussions page of this repo), to avoid double work.

Documentation available here: [https://hassagent.readthedocs.io/en/latest/development/introduction/](https://hassagent.readthedocs.io/en/latest/development/introduction/)

----

Thanks! If you need more info, please join on [Discord](https://discord.gg/nMvqzwrVBU).
