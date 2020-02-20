UiUiUi
------

Description
-----------

UiUiUi is a fairly small and simple UI library designed to work with Monogame.
Philosophically, it is designed to work specifically for the use-case of games,
omitting such things as standard controls - as they usually need to be skinned
anyway. Instead, UiUiui requires you provide your own images of any controls
you want, and their multiple states. UiUiUi can also read layouts from an XML
file.

UiUiUi has been built as part of another project I'm working on. It still lacks
functionality that I consider important in a production ready UI library - such
as touch support, or data driven controls. At the moment, I'm adding
functionality as I need it.

You can already create functional forms, buttons, text boxes and labels.


Versioning
----------

Until v1 is reached, versions may introduce breaking changes. Once v1 is
reached, UiUiUi will employ [semantic versioning](https://semver.org/).


Contributions
-------------

Contributions are welcome. The main repository is the one at
[https://github.com/lkingsford/UiUiUi].

It is requested that either make a new issue, or link to an existing issue,
before making a pull request for new functionality. If you are working on an
issue, it is helpful to claim it on the Issue page.

If you have a bug or a request for functionality, please try to confirm that
it is not already listed as an issue.

Please prefix any commits with the issue number. So, for instance:
```
32: Fix divide by zero on clicking

The control was incorrectly calculating relative position, and crashing.
Changed the relative position to ignore if there are 0 controls.
```

Extended information is not necessary, but is helpful on a complicated commit
that may be unclear as to its reason.

Try to keep commits and pull requests as contained as possible to the issue
that they are fixing.

Contributions are requested to use the [Microsoft C# Coding Standard](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).