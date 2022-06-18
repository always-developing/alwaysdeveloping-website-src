---
title: "Scheduling with GitHub Actions"
lead: "How to schedule a GitHub Action"
Published: 02/08/2022
slug: "08-github-action-schedule"
draft: false
toc: true
categories:
    - DailyDrop
tags:
    - c#
    - .net
    - github
    - action
    - deploy
    - schedule
    - scheduler
    - cron
    
---

## Daily Knowledge Drop

A `GitHub Action` can be configured to executed on a set schedule, and it's as simple as adding a line to the workflow YAML using `Cron syntax`.

As part of the development of this blog, there was a requirement to execute an Action on a schedule - which turned out to be easier than anticipated.

---

## Cron 

Quick intro to `Cron syntax` - if you are already familiar with this you can jump straight to how to [implement in workflow YAML](#yaml-updates)

The `Cron syntax` is used to define a schedule and consists of `5 fields separated by a space, with each field representing a unit of time`.

### Cron syntax

The structure is as follows:

``` powershell
A B C D E

A => minute (0-59)
B => hour (0-23)
C => day of the month (1-31)
D => month (1-12 or JAN-DEC)
E => day of the week (0-6 or SUN-SAT)
```

Each position/unit can have one of the following operators:

- **\*** = any value
- **,** = value list separator
- **\-** = range of value
- **/** step values

### Cron examples

A few examples. 

- `30 * * * *`: executes at minute 30 of every hour of every day
- `15,45 03 * * 1,2`: executes at minute 15 and 45 on hour 03 (3am) every Monday and Tuesday
- `10 18-20 * * SUN`: executes at minute 10 on hour 18, 19 and 20 every Sunday
- `25/10 * * * *`: executes every 10 minutes, starting at minute 25 (so minute 25, 35, 45, 55)

There are resources available online to assist in building up the `Cron` expression (see references below for an example website) 

---

## YAML updates

A `GitHub Action` is a way to automate a process related to a GitHub repository. In my example I use it to build and release this website every day.

The _Action_ definition is stored as YAML as part of the repository, and defines the job(s) to perform as well as when to perform them.

Below is a snippet from this website's YAML file, relating to when the job should be executed.

``` yaml
# Controls when the workflow will run
on:
  # Triggers the workflow on the schedule
  schedule:
    - cron: '30 3 * * 1,2,3,4,5'
  # Allows for manual trigger of workflow
  workflow_dispatch:

# more omitted
```

In the above, we specified that the _Action_ should run every on the schedule specified: `Monday - Friday at 03:30`, and also that the _Action_ can be run manually.

---

## Notes

Scheduling a `Github Actions` is really that simple. _Actions_ are a powerful tool, and the ability to easily schedule them adds to their usefulness - anyone using GitHub repositories which require a workflow of any kind, should investigate how they can be leverage to automate (on a schedule if require) the required workflow.

---

## References
[Crontab Guru](https://crontab.guru)  
[GitHub Action Scheduling](https://docs.github.com/en/actions/using-workflows/events-that-trigger-workflows#schedule)

<?# DailyDrop ?>06: 08-02-2022<?#/ DailyDrop ?>
