# Espresso

Espresso keeps the computer awake, preventing automatic sleep (aka suspend).
It does not prevent the user from putting the computer to sleep manually.

Espresso runs on Windows and Linux.
More platforms can be added by implementing `ISleepInhibitor`.

## Projects

- Espresso.dll contains `ISleepInhibitor` implementations and the static `SleepInhibitor` convenience class.
- Espresso.Console.exe keeps the computer awake from the command line.

## Example

The Espresso library is simple to use in your own program.

    using (ISleepInhibitor inhibitor = SleepInhibitor.StartNew())
    {
        // While inside this block, the computer will not automatically go to sleep.
    }

You may also manually set the `IsInhibited` property.
This is only effective while your program is still running and the `ISleepInhibitor` has not been disposed or finalized.

## License

Espresso is free software licensed under the [Apache License Version 2.0](LICENSE.txt).

## See Also

For a similar application written in python, see [espresso-python](https://github.com/piedar/espresso-python).
