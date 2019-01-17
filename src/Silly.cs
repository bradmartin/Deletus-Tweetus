using System;
using static Utils;

public static class Silly
{
    /// <summary>
    /// Shows the Deletus-Tweetus console print welcome message.
    /// </summary>
    public static void showWelcomeMessage()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        cLog("*************************************************************");
        cLog("*                        Genie                              *");
        cLog("*                                                           *");
        cLog("*                        _.---.__                           *");
        cLog("*                       .'        `-.                       *");
        cLog("*                     /      .--.   |                       *");
        cLog("*                  \\/  / /    | _ /                        *");
        cLog("*                  `\\/|/ _(_)                              *");
        cLog("*                ___ /| _.--'    `.   .                     *");
        cLog("*               \\  `--' .---.    \\ /|                     *");
        cLog("*                 )   `      \\     //|                     *");
        cLog("*                 | __    __ | '/||                         *");
        cLog("*                 |/ \\  / \\      / ||                     *");
        cLog("*                 ||  |  |  \\    \\  |                     *");
        cLog("*                \\|  |  |   /        |                     *");
        cLog("*                __\\@/  |@ | ___\\--'                      *");
        cLog("*               (     / ' `--'  __) |                       *");
        cLog("*              __ > (  .  .--' &'\\                         *");
        cLog("*             /   `--| _ / --'     &  |                     *");
        cLog("*             |                 #. |                        *");
        cLog("*             | q# |                                        *");
        cLog("*             \\              ,ad#'                         *");
        cLog("*               `.________.ad####'                          *");
        cLog("*                 `#####\"\"\"\"\"\"\'\'                    *");
        cLog("*                  `&#\"                                    *");
        cLog("*                   &# \" &                                 *");
        cLog("*                   \"#ba\" *                               *");
        cLog("*                                                           *");
        cLog("*************************************************************");
        Console.ForegroundColor = ConsoleColor.Green;
    }

}