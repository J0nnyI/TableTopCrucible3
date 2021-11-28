

### Content: 
- Progress tracking
- Queing of tasks


### Wording:
Job - A process which is triggered any mean (i.e. filechange or button click) and might take long enough to be executed in the background and have a progress tracker

### Submodules
- Progression
-- Single Responsibility: Track progress for complex tasks
-- No handling of any jobs

- Job Queue
-- Single Responsibility: Queing of Jobs
-- No progress-tracking
-- Queues special commands which have an associated progress tracker to show their execution to the user
