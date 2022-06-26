---
title: "Statiq migration"
lead: "Documenting the process and findings from migrating from Hugo to Statiq"
Published: 12/11/2030
slug: "2022-06-statiq-migration"
draft: true
toc: true
categories:
    - Blog
tags:
    - c#
    - .net
    - blog
    - migration
    - statiq

---

## Why?

### Statiq

First off, a quick introduction to [Statiq](https://www.statiq.dev/) - Statiq is a static site generator, written using C#. It has a flexible, configurable pipline process which takes input artifacts (files, database context, service content) and ouputs _documents_ which can then be used to generate the final static HTML.

### Why Statiq?
As mentioned in the post summary, the previous iteration of the blog was using [Hugo](https://gohugo.io/), a Go based static site generator.
While Hugo was:
- incredible easy to setup the the blog - just download a predefined template and modify some configuration
- incredible easy to use - command line to compile and output the content

it was also:
- difficult  to customise as a C# developer - I am just too unfamilar enough with the Go tools and framework to be able to make any meaningful changes to the predefined templates/processes



- negatives
 - can be slow
 - resources all over the place
 - lack of templates