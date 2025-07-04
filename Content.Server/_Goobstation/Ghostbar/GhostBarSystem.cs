// SPDX-FileCopyrightText: 2024 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Falcon <falcon@zigtag.dev>
// SPDX-FileCopyrightText: 2025 Skubman <ba.fallaria@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <flyingkarii@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Robust.Server.GameObjects;
using Content.Server.Clothing.Systems; // Einstein Engines
using Content.Server.GameTicking;
using Content.Server.GameTicking.Events;
using Content.Server.Players.PlayTimeTracking; // Einstein Engines
using Content.Server.Station.Systems;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Shared.Ghost;
using Content.Server._Goobstation.Ghostbar.Components;
using Content.Server.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Roles;
using Content.Server.Antag.Components;
using Content.Server.Traits; // Einstein Engines
using Content.Shared.Mindshield.Components;
using Content.Shared.Players;
using Content.Shared.Roles.Jobs;
using Robust.Shared.EntitySerialization;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Utility; // Einstein Engines - use JobComponent

namespace Content.Server._Goobstation.Ghostbar;

public sealed class GhostBarSystem : EntitySystem
{
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly GameTicker _ticker = default!;
    [Dependency] private readonly StationSpawningSystem _spawningSystem = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    // Einstein Engines start
    [Dependency] private readonly PlayTimeTrackingManager _playTimeTracking = default!;
    [Dependency] private readonly LoadoutSystem _loadout = default!;
    [Dependency] private readonly TraitSystem _trait = default!;
    // Einstein Engines end

    [ValidatePrototypeId<JobPrototype>] // Einstein Engines - validate job prototypes
    private static readonly List<ProtoId<JobPrototype>> _jobComponents = new()
    {
        "Passenger", "Bartender", "Botanist", "Chef", "Janitor"
    };

    public override void Initialize()
    {
        SubscribeLocalEvent<RoundStartingEvent>(OnRoundStart);
        SubscribeNetworkEvent<GhostBarSpawnEvent>(SpawnPlayer);
        SubscribeLocalEvent<GhostBarPlayerComponent, MindRemovedMessage>(OnPlayerGhosted);
    }

    private ResPath MapPath = new("Maps/_Goobstation/Nonstations/ghostbar.yml");
    private void OnRoundStart(RoundStartingEvent ev)
    {
        if (_mapLoader.TryLoadMap(MapPath, out var mapId, out _))
            _mapSystem.SetPaused((mapId.Value.Owner, null), false);
    }

    public void SpawnPlayer(GhostBarSpawnEvent msg, EntitySessionEventArgs args)
    {
        var player = args.SenderSession;

        if (!_mindSystem.TryGetMind(player, out var mindId, out var mind))
        {
            Log.Warning($"Failed to find mind for player {player.Name}.");
            return;
        }

        if (!_entityManager.HasComponent<GhostComponent>(player.AttachedEntity))
        {
            Log.Warning($"User {player.Name} tried to spawn at ghost bar without being a ghost.");
            return;
        }

        var spawnPoints = new List<EntityCoordinates>();
        var query = EntityQueryEnumerator<GhostBarSpawnComponent>();
        while (query.MoveNext(out var ent, out _))
        {
            spawnPoints.Add(_entityManager.GetComponent<TransformComponent>(ent).Coordinates);
        }

        if (spawnPoints.Count == 0)
        {
            Log.Warning("No spawn points found for ghost bar.");
            return;
        }

        var data = player.ContentData();

        if (data == null)
        {
            Log.Warning($"ContentData was null when trying to spawn {player.Name} in ghost bar.");
            return;
        }

        var randomSpawnPoint = _random.Pick(spawnPoints);
        var randomJob = _random.Pick(_jobComponents);
        var profile = _ticker.GetPlayerProfile(args.SenderSession);
        var mobUid = _spawningSystem.SpawnPlayerMob(randomSpawnPoint, randomJob, profile, null);

        // Einstein Engines start - apply loadouts and traits
        var playTimes = _playTimeTracking.GetTrackerTimes(player);
        var whitelisted = player.ContentData()?.Whitelisted ?? false;

        _loadout.ApplyCharacterLoadout(
            mobUid,
            randomJob,
            profile,
            playTimes,
            whitelisted
        );
        _trait.ApplyTraits(
            mobUid,
            randomJob,
            profile,
            playTimes,
            whitelisted,
            punishCheater: false
        );
        // Einstein Engines end - apply loadouts and traits

        _entityManager.EnsureComponent<GhostBarPlayerComponent>(mobUid);
        _entityManager.EnsureComponent<MindShieldComponent>(mobUid);
        _entityManager.EnsureComponent<AntagImmuneComponent>(mobUid);
        _entityManager.EnsureComponent<IsDeadICComponent>(mobUid);

        if (mind.Objectives.Count == 0)
            _mindSystem.WipeMind(player);
        mindId = _mindSystem.CreateMind(data.UserId, profile.Name).Owner;
        _mindSystem.TransferTo(mindId, mobUid, true);
    }

    private void OnPlayerGhosted(EntityUid uid, GhostBarPlayerComponent component, MindRemovedMessage args)
    {
        _entityManager.DeleteEntity(uid);
    }
}

