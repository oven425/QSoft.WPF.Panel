# QSoft.WPF.Panel 
## Quick start
1. install from [nuget](https://www.nuget.org/packages/QSoft.WPF.Panel)
2. add qpanel into xaml
```xml
<Window x:Class="WpfApp_FlexPanelT.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WpfApp_FlexPanelT"
        mc:Ignorable="d"
        xmlns:qpanel="clr-namespace:QSoft.WPF.Panel;assembly=QSoft.WPF.Panel"
        Title="FlexPanel test site" Height="450" Width="800">

        <qpanel:FlexPanel>
            <Button Width="100">Test1</Button>
            <Button Width="100">Test2</Button>
            <Button Width="100">Test3</Button>
        </qpanel:FlexPanel>

</Window>
```
## Direction: Row,Column,RowReverse,ColumnReverse
```xml
<qpanel:FlexPanel FlexDirection="Row">
    <Button>Test1</Button>
    <Button>Test2</Button>
    <Button>Test3</Button>
</qpanel:FlexPanel>
```

## JustifyContent: Start, End, Center, SpaceAround, SpaceBetween, SpaceEvenly
```xml
<qpanel:FlexPanel JustifyContent="Start">
    <Button>Test1</Button>
    <Button>Test2</Button>
    <Button>Test3</Button>
</qpanel:FlexPanel>
```

## AlignItems: Start, End, Center, Stretch
```xml
<qpanel:FlexPanel AlignItems="Start">
    <Button>Test1</Button>
    <Button>Test2</Button>
    <Button>Test3</Button>
</qpanel:FlexPanel>
```

## FlexWrap: NoWrap, Wrap, WrapReverse
```xml
<qpanel:FlexPanel FlexDirection="Row" FlexWrap="Wrap" Gap="8">
    <Button qpanel:FlexPanel.Basis="150">Test1</Button>
    <Button qpanel:FlexPanel.Basis="150">Test2</Button>
    <Button qpanel:FlexPanel.Basis="150">Test3</Button>
</qpanel:FlexPanel>
```
* `NoWrap` (default): all items stay on one line, `Shrink` makes them fit.
* `Wrap`: an item that doesn't fit moves to a new line (a new column for `Column`/`ColumnReverse`). `Grow` and `Shrink` are resolved for every line on its own.
* `WrapReverse`: like `Wrap`, but the lines are stacked from the other side of the cross axis.
* `Gap` is the space between the items of a line and the space between the lines.
* Lines are only broken when the panel has a limited length on the main axis. In a `ScrollViewer` that scrolls along the main axis all items stay on one line.

## AlignContent: Stretch, Start, End, Center, SpaceBetween, SpaceAround, SpaceEvenly
> Places the lines on the cross axis, so it only has an effect when `FlexWrap` is `Wrap` or `WrapReverse`. The default `Stretch` shares the free space among the lines.
```xml
<qpanel:FlexPanel FlexWrap="Wrap" AlignContent="SpaceBetween" Gap="8">
    <Button qpanel:FlexPanel.Basis="150">Test1</Button>
    <Button qpanel:FlexPanel.Basis="150">Test2</Button>
    <Button qpanel:FlexPanel.Basis="150">Test3</Button>
</qpanel:FlexPanel>
```
## Gap
```xml
<qpanel:FlexPanel FlexDirection="Row" Gap="8">
    <Button>Test1</Button>
    <Button>Test2</Button>
    <Button>Test3</Button>
</qpanel:FlexPanel>
```

## AlignSelf: Auto, Start, End, Center, Stretch
```xml
<qpanel:FlexPanel FlexDirection="Row" Gap="8">
    <Button qpanel:FlexPanel.AlignSelf="Start">Test1</Button>
    <Button qpanel:FlexPanel.AlignSelf="End">Test2</Button>
    <Button qpanel:FlexPanel.AlignSelf="Center">Test3</Button>
</qpanel:FlexPanel>
```
## Grow
> use Grow, not set Width/Hieght when Direction Row/Column
```xml
<qpanel:FlexPanel FlexDirection="Row" Gap="8">
    <Button qpanel:FlexPanel.Grow="1">Test1</Button>
    <Button>Test2</Button>
    <Button>Test3</Button>
</qpanel:FlexPanel>
```
## Basis: 0~double.PositiveInfinity
```xml
<qpanel:FlexPanel FlexDirection="Row" Gap="8">
    <Button qpanel:FlexPanel.Basis="100">Test1</Button>
    <Button qpanel:FlexPanel.Basis="200">Test2</Button>
    <Button qpanel:FlexPanel.Basis="300">Test3</Button>
</qpanel:FlexPanel>
```
## Compare with CSS flexbox
`WpfApp_FlexT` lays a test case out with `FlexPanel` (left) and with the flexbox of a browser, in a WebView2 (right), and lists the items whose position or size differ by more than 1px (bottom). The test cases are the JSON files in `WpfApp_FlexT\TestCase`. As there are hundreds of them, narrow the list down first: pick a category (the part of the file name before its first `_`, such as `Wrap` or `Fuzz`) and/or type search keywords (separated by spaces; each one has to occur in the file name or the description). The search box suggests the settings that can complete the word being typed, such as `Direction=Column`, each with the number of test cases that are left when it is taken: pick one with the arrow keys and Enter or Tab, or with the mouse; Esc closes the list and the down arrow opens it again. A keyword that is a whole suggestion matches that setting only (`Direction=Column` does not match `Direction=ColumnReverse`). Then choose the test case in the ComboBox, or step through the list with the previous/next buttons. The settings of the chosen test case, those of the panel and of every item, are shown in the box under the ComboBox.

Known differences:
* WPF measures an item with the room the panel has. An item whose content is larger than the panel itself (for example text taller than a `Row` panel, or an item without `Basis` that is longer than the panel) is cut down to the panel's size, whereas CSS lets it keep its content size and overflow. An item with an explicit `Width`/`Height` (or `MinWidth`/`MinHeight`) keeps that size, like in CSS.
* An item without `Basis` that has a `MinWidth`/`MinHeight` larger than its content starts from that minimum, whereas CSS starts from its content size, so `Grow` and `Shrink` share the space a bit differently.
## Refrences
[Flexbox test](https://oven425.github.io/my-flex-app)


