# WigiDash Widget Template

Built against [WigiDash SDK](https://github.com/ElmorLabs-WigiDash/WigiDashWidgetFramework) for the [WigiDash Panel](https://www.gskill.com/product/412/415/1702982997/WigiDash). Which is shipped in the WigiDash Manager software

Last tested against Wigidash Manager v1.1.9073.39614

# Getting Started

* Set the 'Toggable' bool in CleverWidgetBase.cs based on your usage
* Overwrite the UI in MyWidgetControls.xaml
* Implement your own drawing code in ThreadDrawTask and click handling functionality ClickEvent of MyWidget.cs
* Add preview_X_Y.pngs, thumb.png or implement the overriden function in the MyWidget classes 

# Baseclass functionality

Overlay text, images and gifs which are resizeable, rotatable and repositionable for 2 'Toggleable' states

# GUIDs

if you're wanting to share your widget, then there is a list of GUIDs you should recreate at the top of MyWidget.cs

# Special Thanks

to ElmorLabs & G.Skill for creating this nifty product