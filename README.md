# FaraBank

FaraBank is my own PFM, Personal financial manager

Here I'll explain the development journey

# No1.NHibernateNodaTime

As I use NH for persistence and NodaTime for date and time, I searched for a library to store all NodaTime types including Instant, ZonedDateTime, etc.
I didn't find a complete library, so I developed it.
You can find its repo in [https://github.com/behzadian/No1.NHibernateNodaTime]

# No1.EnvBasedEndpoints

In the development, for testing exception handling, I needed an endpoint which throws exception. But I didn't want it to be available on production environment. So I developed No1.EnvBasedEndpoints. 
This library provides you multiple attributes that you can apply on your controllers and endpoints to include or exclude them in specific environments.
For more information, you can open related repo [https://github.com/behzadian/No1.EnvBasedEndpoints]


# CORS testing
CORS is one of the hardest easiest things to config in server side development. Hopefully asp.net has good support for it and I just added my own configuration class and then apply it. Also, I developed 2 tests to check for it.

# Error handling
Error handling is one of most important aspects of an application maintenance.

In FaraBank I wanted erros to be:

- Standard
- Detailed & readable on development
- Minimal and fast on production

## Find and update the outdated deps:

```
dotnet tool install --global dotnet-outdated-tool   
dotnet outdated        # to show outdated deps
dotnet outdated -u   # to update deps
```

## Add repo hooks:

```shell
git config --local core.hooksPath .husky
```
