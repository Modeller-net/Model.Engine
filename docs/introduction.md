# Modeller

The model tool is used to generate code and documentation for a project.  It is a command line tool that takes a
definition file and generates the code and documentation for the project. The tool is written in C# and is available
as a .NET Core global tool.

There are two ways to create definitions and these depend on the audience using it.  If the audience is non-technical
then the YAML format is the best option as this doesn't rely on code changes and allows for generation of code by
mearly changing a text file.

If the audience is technical then the approved method is to create a `Definition.dll` by creating a project using the
supplied fluent language to define the definition.  This can then be persisted to files and used to generate the code.

The point of truth will always be the fluent language as this is controlled by ensures the developers have included
_user-defined_ logic in the generated code and will ensure the manual changes are checked into source control for
future reference.

## Definition Files

A definition file describes the project that has been modelled, and allows for multiple versions to exist as well as
manual changes to the files.

Below is the example definition for a test project we use to verify the code gen system

``` yaml
company: JJRichards
project: NewBranch
version: 1.0
summary: Status information and summary goes here
```

A definition file must be present and should have an extension of `.def`.

All definitions must have at least one entity and these must be contained in a sub-folder named **entities** in the
same folder as the `.def` file.

### Entities

In the context of databases and information systems, an entity is a distinct object or concept about which data can be stored.
In entity-relationship modeling, entities are represented as tables, and relationships between entities are depicted to
illustrate how they are connected.

Entities must be defined in a YAML file with the `.entity` extension, placed in a sub-folder within the **entities** folder,
and named according to the entity name.  For example, an entity named `Status` would be defined in a file named `Status.entity`.

``` yaml
name: Status
fields:
- name: Name
  dataType:
    type: string
    maxLength: 20
    minLength: 3
  businessKey: true
  summary: The default display text for the status.
- name: CreatedBy
  dataType:
    type: string
    maxLength: 100
  summary: The user that created the record.
- name: CreatedOn
  dataType: DateTimeOffset
  summary: The date and time the record was created.
- name: Active
  dataType: bool
  default: true
  summary: Determines is the record is active or not.
  remarks: '(Default: true)'
supportCrud: All
summary: Status values are used to determine the current state of a case.
```
#### Entity Keys

In addition to defining entities, you should also define the keys for each entity.  These are defined in a YAML file
format with the extension `.key`.  They are also placed in the entity folder and named using the entity name.
For example, an entity named `Status` would have a key defined in a file named `Status.key`.

``` yaml
keyFields:
- name: StatusId
  dataType: int
  summary: The unique identifier for the status.
summary: Unique key for the status
```
### Enumerations

Enumerations must be defined in a YAML file with the `.enum` extension, placed in a sub-folder named **enumerations**
off the root folder, and named according to the enumeration name.  For example, an enumeration named `StatusType` would
be defined in a file named `StatusType.enum`.

``` yaml
name: StatusType
values:
- name: Active
  value: 1
  summary: Active status type information and summary goes here
- name: Cancelled
  value: 2
  summary: Cancelled status type information and summary goes here
- name: Closed
  value: 3
  summary: Closed status type information and summary goes here
summary: Status type information and summary goes here
```

### Services

Services must be defined in a YAML file with the `.service` extension, placed in a sub-folder named **services** off
the root folder, and named according to the service name. For example, a service named `StatusService` would be
defined in a file named `StatusService.service`.

``` yaml
name: StatusService
entities:
- Status
- StatusReason
summary: Status service information and summary goes here
```
Services define which entities they use and are used to group entities together.  This helps during generation for example
when generating a database, all entities in a service will be placed in the same schema.  It also allows for the addition
of reference data that is owned by other services and determines the rules for how the reference data is cached.

### Endpoints

Endpoints must be defined in a YAML file with the `.endpoint` extension, placed in a sub-folder named **endpoints** off
the root folder, and named according to the endpoint name.  For example, an endpoint named `AddStatusEndpoint` would be
defined in a file named `AddStatusEndpoint.endpoint`.

``` yaml
routeTemplate: /api/status
name: AddStatusEndpoint
verb: Post
request:
  name: AddStatusRequest
  mediaType: application/json
  body:
    mediaType: application/json
    fields:
    - name: Name
      dataType:
        type: string
        maxLength: 20
        minLength: 3
      summary: The default display text for the status.
  summary: Add new Status request information and summary goes here
summary: Add new Status endpoint information and summary goes here
```
Endpoints define a REST endpoint, which includes the route template, the HTTP verb, and the request and response types.
The request and response types can potentially include a complex structure, comprising the following elements:
- Query Paramters - These are defined in the `queryParameters` section of the endpoint definition.
- Body - This is a complex structure that is defines a model that is passed in the body of the request/response.
- Headers - These are defined in the `headers` section of the endpoint definition.  These are Key/Value pairs that are
  placed in the request header.

## Fluent Language

The fluent language is used to define the definition and is the preferred method for creating definitions.  It is a
fluent language that is used to create a `Definition.dll` that is then used to generate the code and documentation.
