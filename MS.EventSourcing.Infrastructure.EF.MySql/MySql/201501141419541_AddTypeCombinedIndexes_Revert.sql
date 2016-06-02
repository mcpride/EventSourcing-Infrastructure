alter table `SnapshotStream` drop index `IdxAggregateRootIdSnapshotType`;
alter table `EventStream` drop index `IdxAggregateRootIdAggregateType`;
DELETE FROM `__MigrationHistory` WHERE `MigrationId` = '201501141419541_AddCombinedAggregateIdTypeIndexesToStreams';
