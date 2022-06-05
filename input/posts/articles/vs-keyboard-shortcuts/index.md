---
title: "Useful Visual Studio keyboard shortcuts"
lead: "Be up to 20% more productive!"
Published: 10/25/2021
slug: "vs-keyboard-shortcuts"
draft: false
toc: true
categories:
    - Blog
tags:
    - c#
    - .net
    - visualstudio
    - ide
    - productivity
---

## Why learn keyboard shortcuts?

So I confess I have no empirical evidence to backup the `up to 20% more productive` claim. That number is just made up, but with years of experience on my side, it honestly feels fairly accurate to me!  
  
Having often been involved in assisting, troubleshooting and debugging code with fellow developers, it is apparent that the developers which are familiar with their IDE, and make use of the shortcuts, _generally_ (but not always!) resolve tasks quicker and more efficiently than those that don't.

Every hand reach for the mouse, every unnecessary cursor movement, every menu click is _potentially_ a waste of time. It's time two hands are not on the keyboard writing code. The more shortcuts a person is familiar with, the more the unnecessary time wasting can be minimized. Thus, more programming time and more productivity!
  
- Is there anything wrong with not making use of keyboard shortcuts? __Definitely not.__ 
- Does not using keyboard shortcuts make a person any less of a developer? __Definitely not.__ 
- Could using keyboard shortcuts make a person more productive? __Definitely yes.__

## Visual Studio shortcuts to learn

Below is a list of useful `Visual Studio` shortcuts I use the most often and find the more useful in my day to day development. This is by no means an exhaustive list - however I have no doubt a massive benefit can be gained by learning just a few of these.  
  
Some of these shortcuts are not just Visual Studio shortcuts, and also be leveraged in other applications (such as VS Code).


<?# InfoBlock ?>

- `+` indicates a combination of keys is to be pressed to perform the action.  
In some cases (like the example below), the final key can be tapped to perform the action multiple times (while still holding down the initial two keys)  
E.g. `Ctrl + Shift + -`: _Ctrl_ and _Shift_ can be held down at the same time while the _-_ key is pressed multiple times.  

- `,` is used to indicate a sequence of keys is to be pressed.  
E.g. `Ctrl + M, O`: _Ctrl_ is held down, while _M_ is pressed and then _O_ is pressed.

<?#/ InfoBlock ?>

### View shortcuts

- `Ctrl + -` and `Ctrl + Shift + -`: __Navigate backwards and forwards__ 🧭  
Moves the cursor, backwards and forwards through the history of visited cursor locations in file(s). This is incredibly useful especially when used in conjunction with the _Go to Definition_ / `Ctrl + F12` function.

- `Ctrl + .`: __Quick actions and refactoring__ 💡  
When the cursor is over a block of code, this shortcut will bring up the quick actions and refactoring (Lightbulb or screwdriver icon) menu

- `Ctrl + Spacebar`: __Trigger Intellisense__ 

### Editor shortcuts

- `Ctrl + ←` and `Ctrl + →`: __Moves cursor one word to the left or right__ ⬅️➡️  
Great when used in combination with the _Shift_ key (e.g. `Ctrl + Shift + →`) to highlight/select entire word(s).

- `Ctrl + Del`: __Delete an entire line__ 🚫  
When you dislike your code and you want it gone quickly.

- `Ctrl + M, O`: __Collapse to definitions__ 📄  
Collapse all methods, regions, comment blocks etc in the current document.

- `Ctrl + F`: __Find in current file__ 🔍  
Defaults to search in only the current document, but this can be changed to include more documents (e.g. entire solution)

- `Ctrl + Shift + F`: __Find in all files__ 🔍🔍  
Opens the _Find in files_ dialog. Defaults to search the entire solution, but this can be changed to include less documents (e.g. current document)

- `Ctrl + H`: __Replace in current file__ 📑  
Defaults to search in only the current document, but this can be changed to include more documents (e.g. entire solution)

- `Ctrl + Shift + H`: __Replace in all files__ 📑📑  
Opens the Replace in files_ dialog. Defaults to replace in the entire solution, but this can be changed to include less documents (e.g. current document)

- `Ctrl + K, C` and `Ctrl + K, U`: __Comments and uncomment code selection__ 📜  
Comment and uncomment code selection. If no selection is made, the line of code the cursor is current on will be commented/un-commented.

### Refactor shortcuts

- `'ctor', Tab, Tab`: __Constructor creation__ 🏗️  
This is a prebuilt code snippet and not really a keyboard shortcut. This will create a default parameter-less constructor for the current class

- `Ctrl + R, R`: __Rename__ 💬  
Allows for the rename of a class, method, variable etc. as well as all usages of said code.  Place the cursor on a method name, for example, press `Ctrl + R, R`, type in the new name and hit enter. The method name and all usages of the name have now been renamed.

- `Ctrl + R, M`: __Extract to method__ 📤  
Create a new method containing the selected code, and invoke the new method from the current code location. Great for code clean up.

- `Ctrl + R, G`: __Remove and sort _usings___ ⛔
Performing this shortcut anywhere in a document will remove any unused _usings_ in the file, as well as sort the remaining ones alphabetically.

### Build shortcuts

- `F5`: __Build and start application with the _debugger attached___. 🐛  
Breakpoints will pause code execution, code can be stepped through, etc.

- `Ctrl + F5`: __Build and start application _without the debugger attached___. 🏃‍♂️  
No debug symbols will be loaded, so breakpoints will not be hit. Most often used when:  
    1. Running multiple dependent services/applications in the same solution (without the need for debugging all the projects)
    2. Running benchmarking (using BenchmarkDotNet)
    
### Code quality shortcuts

- `///`: __Adds comments__ 🧾  
Used above a method or class to create and partially auto populate the comments.  
These comments can be used to generate an XML document file (especially useful for library authors)

## Conclusion

It does take a conscious effort when starting to actually slow down, lookup the shortcut to be used and force yourself to use it. But adaption happens quickly, and before you know it you'll be using the keyboard shortcuts without even realising it.

It is an on-going learning process - if you find yourself performing the same time consuming action over and over in the IDE, consider investigating and learning the shortcut. 

The list of [`VS2019 Keyboard shortcuts`](https://docs.microsoft.com/en-us/visualstudio/ide/default-keyboard-shortcuts-in-visual-studio?view=vs-2019). (this mostly apply to VS2017 and VS2022 as well)

Use shortcuts. Be more productive.
