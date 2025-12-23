-- simple webhooks to ensure guarateed data delivery in case of server shutdowns.
-- logic allows for max retries of 6 (increment at 2 seconds ) 
-- manual review if POST requests fail after max no. of retries 
-- tables to create:
Use [Webhooks-Delivery]
go

drop table if exists [Webhooks-Delivery].WebhookEvents
go

drop table if exists [Webhooks-Delivery].WebhookEPs
go

create table WebhookEPs (
id int primary key identity not null,
Name nvarchar(100) not null,
Url nvarchar(500) not null,
Secret nvarchar(256) not null,
IsActive bit not null default 1,
CreatedAt datetime not null default getutcdate()
);
go

create table WebhookEvents (
id int primary key identity not null,
EventPointID int not null,
EventType nvarchar(100) not null,
Payload nvarchar(max) not null,
Status tinyint not null default 0,
--status : 0 =pending, 1= delivered , 2=failed, 3=deadletter
AttemptCount smallint not null default 0,
NextAttemptAt datetime2,
LastAttemptAt datetime2,
LastResponsecode int,
LastErrorMessage nvarchar(500),
ProcessingLockUntil Datetime2,
CreatedAt Datetime2 not null default getutcdate(),
DeliveredAt datetime2,
constraint fk_webhookEPs_to_webhookEvents Foreign key (EventPointID) 
references WebhookEps(id)
on delete cascade
);

go

create index ix_events_pending
on WebhookEvents(Status, NextAttemptAt)
Where Status = 0;
go
