// SPDX-FileCopyrightText: 2024 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 gluesniffler <linebarrelerenthusiast@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Utility;

namespace Content.Client._Shitmed.Xenonids.UI;

[GenerateTypedNameReferences]
[Virtual]
public partial class XenoChoiceControl : Control
{
    public XenoChoiceControl() => RobustXamlLoader.Load(this);

    public void Set(string name, Texture? texture)
    {
        NameLabel.SetMessage(name);
        Texture.Texture = texture;
    }

    public void Set(FormattedMessage msg, Texture? texture)
    {
        NameLabel.SetMessage(msg);
        Texture.Texture = texture;
    }
}
