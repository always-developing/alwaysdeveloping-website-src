---
title: "Git for beginners"
lead: "Git terminology, commands and Visual Studio and VS Code functionality for beginners"
Published: 06/11/1900
slug: "git-beginners"
Excluded: true
toc: true
categories:
    - Blog
tags:
    - c#
    - .net
    - git
    - beginners
    - guide
    - devops
    - terminology
    - visual studio
    - commands
---

## Git in a nutshell
Git has for a long time been the defacto standard for source control, but in recent years has seen an increase in popularity and has become even more widely adopted, from open-source to startups and large corporate environments. As a beginner or experienced developer in any of these environments, at least a basic understanding of how Git works, as well as the basic commands (or how to use the relevant Visual Studio tools) is now essential.

So what is Git, and what makes it different. In short, Git is just a `version control system, keeping a history of changes made to file(s)`. However it does this **very well**, and makes it **easy to do**.  
It does have a bit of a learning curve, but once the curve is overcome, the day to day usage becomes fluent and natural.

## How to use Git (in plain English)
### Hosting
The first step is to create a place to store your `repository`.  
A `repository` is a container which holds files, as well as keeps a history of and tracks any and all changes made to these files. My recommendation is to create a **free account on [Github.com](http://www.github.com)** which allows for public and private repositories to be created. 

{{<block-info>}}
**Github** and **Git** are not the same thing.  
`Git` is a type of repository, and the underlying tool-set to operate on the repository.  
`GitHub` is a website, which provides functionality to store these Git repositories (with value added functionality around the repository)
{{</block-info>}}

### Connecting to the host 

### Creating a new repository

### Using an existing repository

The first step is to `clone` the repository (how to do this and all other commands are described below) - this means _making a copy of the repository locally on your hard-drive, including the physical files in the repository as well as the full history of any and all changes previously made to the files in the repository._

### Stage

### Commit

## Benefit highlights
There are a number of benefits and advantages to using Git. These are my highlights (for full context see the below commands)
- **`Git is distributed`**: If you have a Git repository locally, you have a _full history_ of changes made to that repository locally. No requirement to connect to a central server to view history.
- **`Git commits are local`**: When code is committed (checked-in), it is initially done to your local repository: no changes are visible to anyone else (yet). This means as long as you commit frequently, its trivial to go back to a previous version of the code if need be.
- **`Git branching is trivial`**: especially coming from using SVN historically and TFVC more recently (more centralized version control systems) where branching was never widely used due to its complexity.

# Commands

Below are a list of the basic commands to get up and running with Git. I am using GitHub the examples - GitHub is not Git. Git is a source control repository type, while github is a host of git repositories.


Init
stage
commit
new branch
push
pull
PR
stash