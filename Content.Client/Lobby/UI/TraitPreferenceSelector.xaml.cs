// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <77995199+DEATHB4DEFEAT@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Rosycup <178287475+Rosycup@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using System.Text;
using Content.Client.Players.PlayTimeTracking;
using Content.Client.Stylesheets;
using Content.Shared.Customization.Systems;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.Traits;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Lobby.UI;


[GenerateTypedNameReferences]
public sealed partial class TraitPreferenceSelector : Control
{
    public TraitPrototype Trait { get; }

    public bool Valid;
    private bool _showUnusable;
    public bool ShowUnusable
    {
        get => _showUnusable;
        set
        {
            _showUnusable = value;
            Visible = Valid || _showUnusable;
            PreferenceButton.RemoveStyleClass(StyleBase.ButtonDanger);
            PreferenceButton.AddStyleClass(Valid ? "" : StyleBase.ButtonDanger);
        }
    }

    public bool Preference
    {
        get => PreferenceButton.Pressed;
        set => PreferenceButton.Pressed = value;
    }

    public event Action<bool>? PreferenceChanged;

    public TraitPreferenceSelector(TraitPrototype trait, JobPrototype highJob, HumanoidCharacterProfile profile,
        IEntityManager entityManager, IPrototypeManager prototypeManager, IConfigurationManager configManager,
        CharacterRequirementsSystem characterRequirementsSystem, JobRequirementsManager jobRequirementsManager)
    {
        RobustXamlLoader.Load(this);

        Trait = trait;

        // Create a checkbox to get the loadout
        PreferenceButton.AddChild(new BoxContainer
        {
            Children =
            {
                new Label
                {
                    Text = trait.Points.ToString(),
                    StyleClasses = { StyleBase.StyleClassLabelHeading },
                    MinWidth = 32,
                    MaxWidth = 32,
                    ClipText = true,
                    Margin = new Thickness(0, 0, 8, 0),
                },
                new Label
                {
                    Text = Loc.GetString($"trait-name-{trait.ID}"),
                    Margin = new Thickness(8, 0, 0, 0),
                },
            },
        });
        PreferenceButton.OnToggled += OnPrefButtonToggled;

        var tooltip = new StringBuilder();
        // Add the loadout description to the tooltip if there is one
        var desc = Loc.GetString($"trait-description-{trait.ID}");
        if (!string.IsNullOrEmpty(desc) && desc != $"trait-description-{trait.ID}")
            tooltip.Append(desc);


        // Get requirement reasons
        characterRequirementsSystem.CheckRequirementsValid(
            trait.Requirements, highJob, profile, new Dictionary<string, TimeSpan>(),
            jobRequirementsManager.IsWhitelisted(), trait,
            entityManager, prototypeManager, configManager,
            out var reasons);

        // Add requirement reasons to the tooltip
        foreach (var reason in reasons)
            tooltip.Append($"\n{reason}");

        // Combine the tooltip and format it in the checkbox supplier
        if (tooltip.Length > 0)
        {
            var formattedTooltip = new Tooltip();
            formattedTooltip.SetMessage(FormattedMessage.FromMarkupPermissive(tooltip.ToString()));
            PreferenceButton.TooltipSupplier = _ => formattedTooltip;
        }
    }

    private void OnPrefButtonToggled(BaseButton.ButtonToggledEventArgs args)
    {
        PreferenceChanged?.Invoke(Preference);
    }
}
