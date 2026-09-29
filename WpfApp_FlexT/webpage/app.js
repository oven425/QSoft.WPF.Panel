/*
  Lays out #box from the FlexPanelTestData (WpfApp_FlexT\MainWindow.xaml.cs) that WPF posts with
  CoreWebView2.PostWebMessageAsJson, so CSS flexbox can be compared with QSoft.WPF.Panel.FlexPanel.
*/
(() => {
  'use strict';

  // FlexPanel enums in declaration order, so both enum names and numeric values are understood.
  const cssFlexDirection = {
    Row: 'row',
    RowReverse: 'row-reverse',
    Column: 'column',
    ColumnReverse: 'column-reverse',
  };
  const cssJustifyContent = {
    Start: 'flex-start',
    End: 'flex-end',
    Center: 'center',
    SpaceAround: 'space-around',
    SpaceBetween: 'space-between',
    SpaceEvenly: 'space-evenly',
  };
  const cssAlignItems = {
    Start: 'flex-start',
    End: 'flex-end',
    Center: 'center',
    Stretch: 'stretch',
  };

  const box = document.getElementById('box');
  const itemTemplate = document.getElementById('item-template');

  // Reads a property by its C# name; the camelCase name is accepted too.
  function read(source, name) {
    if (source === null || typeof source !== 'object') return undefined;
    return source[name] ?? source[name[0].toLowerCase() + name.slice(1)];
  }

  // Unknown values fall back to the first enum member, which is the FlexPanel default.
  function toCss(map, value) {
    const names = Object.keys(map);
    const name = typeof value === 'number'
      ? names[value]
      : names.find(n => n.toLowerCase() === String(value).toLowerCase());
    return map[name ?? names[0]];
  }

  // FlexPanel treats negative values as 0.
  function toNumber(value) {
    const number = Number(value);
    return Number.isFinite(number) ? Math.max(number, 0) : 0;
  }

  function createItem(child, index) {
    const element = itemTemplate.content.firstElementChild.cloneNode(true);
    element.querySelector('.item-label').textContent = `index: ${index}`;
    const basis = toNumber(read(child, 'Basis'));
    Object.assign(element.style, {
      flexGrow: String(toNumber(read(child, 'Grow'))),
      flexShrink: String(toNumber(read(child, 'Shrink'))),
      // FlexPanel keeps the child's desired size when Basis is 0.
      flexBasis: basis > 0 ? `${basis}px` : 'auto',
    });
    return element;
  }

  function render(data) {
    const padding = read(data, 'Padding');
    Object.assign(box.style, {
      flexDirection: toCss(cssFlexDirection, read(data, 'Direction')),
      justifyContent: toCss(cssJustifyContent, read(data, 'JustifyContent')),
      alignItems: toCss(cssAlignItems, read(data, 'AlignItems')),
      gap: `${toNumber(read(data, 'Gap'))}px`,
      paddingLeft: `${toNumber(read(padding, 'Left'))}px`,
      paddingTop: `${toNumber(read(padding, 'Top'))}px`,
      paddingRight: `${toNumber(read(padding, 'Right'))}px`,
      paddingBottom: `${toNumber(read(padding, 'Bottom'))}px`,
    });

    const childs = read(data, 'Childs');
    box.replaceChildren(...(Array.isArray(childs) ? childs : []).map(createItem));
  }

  // PostWebMessageAsJson delivers an object, PostWebMessageAsString delivers JSON text.
  window.chrome?.webview?.addEventListener('message', event => {
    let data = event.data;
    if (typeof data === 'string') {
      try {
        data = JSON.parse(data);
      } catch {
        return;
      }
    }
    if (data !== null && typeof data === 'object') render(data);
  });
})();
