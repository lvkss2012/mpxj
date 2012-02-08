﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sf.mpxj;
using net.sf.mpxj.reader;
using java.util;

namespace MpxjQuery
{
    class MpxjQuery
    {
        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">command line arguments</param>
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                {
                    System.Console.WriteLine("Usage: MpxQuery <input file name>");
                }
                else
                {
                    query(args[0]);
                }
            }

            catch (Exception ex)
            {
                System.Console.WriteLine(ex.StackTrace);
            }

        }

        /// <summary>
        /// This method performs a set of queries to retrieve information
        /// from the an MPP or an MPX file.
        /// </summary>
        /// <param name="filename">name of the project file</param>
        private static void query(String filename)
        {
            ProjectReader reader = ProjectReaderUtility.getProjectReader(filename);
            ProjectFile mpx = reader.read(filename);

            System.Console.WriteLine("MPP file type: " + mpx.getMppFileType());

            listProjectHeader(mpx);

            listResources(mpx);

            listTasks(mpx);

            listAssignments(mpx);

            listAssignmentsByTask(mpx);

            listAssignmentsByResource(mpx);

            listHierarchy(mpx);

            listTaskNotes(mpx);

            listResourceNotes(mpx);

            listRelationships(mpx);

            listSlack(mpx);

            listCalendars(mpx);
        }

        /// <summary>
        /// Convenience method to convert a Java Date into a .net DateTime.
        /// </summary>
        /// <param name="javaDate">Java Date instance</param>
        /// <returns>DateTime instance</returns>
        private static DateTime ToDateTime(Date javaDate)
        {
            DateTime result = new DateTime(621355968000000000 + (javaDate.getTime() * 10000));
            return result;
        }

        /// <summary>
        /// Convenience method to convert a Java Collection into an enumerable instance.
        /// </summary>
        /// <param name="javaCollection">Java Collection instance</param>
        /// <returns>enumerable object</returns>
        private static EnumerableCollection ToEnumerable(Collection javaCollection)
        {
            return new EnumerableCollection(javaCollection);
        }

        /// <summary>
        /// Reads basic summary details from the project header.
        /// </summary>
        /// <param name="file">project file</param>
        private static void listProjectHeader(ProjectFile file)
        {
            ProjectHeader header = file.getProjectHeader();
            Date startDate = header.getStartDate();
            Date finishDate = header.getFinishDate();
            String formattedStartDate = startDate == null ? "(none)" : ToDateTime(startDate).ToString();
            String formattedFinishDate = finishDate == null ? "(none)" : ToDateTime(finishDate).ToString();

            System.Console.WriteLine("Project Header: StartDate=" + formattedStartDate + " FinishDate=" + formattedFinishDate);
            System.Console.WriteLine();
        }

        /// <summary>
        /// This method lists all resources defined in the file.
        /// </summary>
        /// <param name="file">project file</param>
        private static void listResources(ProjectFile file)
        {
            foreach (Resource resource in ToEnumerable(file.getAllResources()))
            {
                System.Console.WriteLine("Resource: " + resource.getName() + " (Unique ID=" + resource.getUniqueID() + ") Start=" + resource.getStart() + " Finish=" + resource.getFinish());
            }
            System.Console.WriteLine();
        }

        /// <summary>
        /// This method lists all tasks defined in the file.
        /// </summary>
        /// <param name="file">project file</param>
        private static void listTasks(ProjectFile file)
        {
            String startDate;
            String finishDate;
            String duration;
            Date date;
            Duration dur;

            foreach (Task task in ToEnumerable(file.getAllTasks()))
            {
                date = task.getStart();
                if (date != null)
                {
                    startDate = ToDateTime(date).ToString();
                }
                else
                {
                    startDate = "(no date supplied)";
                }

                date = task.getFinish();
                if (date != null)
                {
                    finishDate = ToDateTime(date).ToString();
                }
                else
                {
                    finishDate = "(no date supplied)";
                }

                dur = task.getDuration();
                if (dur != null)
                {
                    duration = dur.toString();
                }
                else
                {
                    duration = "(no duration supplied)";
                }

                String baselineDuration = task.getBaselineDurationText();
                if (baselineDuration == null)
                {
                    dur = task.getBaselineDuration();
                    if (dur != null)
                    {
                        baselineDuration = dur.toString();
                    }
                    else
                    {
                        baselineDuration = "(no duration supplied)";
                    }
                }

                System.Console.WriteLine("Task: " + task.getName() + " ID=" + task.getID() + " Unique ID=" + task.getUniqueID() + " (Start Date=" + startDate + " Finish Date=" + finishDate + " Duration=" + duration + " Baseline Duration=" + baselineDuration + " Outline Level=" + task.getOutlineLevel() + " Outline Number=" + task.getOutlineNumber() + " Recurring=" + task.getRecurring() + ")");
            }
            System.Console.WriteLine();
        }

        /// <summary>
        /// This method lists all tasks defined in the file in a hierarchical format, 
        /// reflecting the parent-child relationships between them.
        /// </summary>
        /// <param name="file">project file</param>
        private static void listHierarchy(ProjectFile file)
        {
            foreach (Task task in ToEnumerable(file.getChildTasks()))
            {
                System.Console.WriteLine("Task: " + task.getName());
                listHierarchy(task, " ");
            }

            System.Console.WriteLine();
        }

        /// <summary>
        /// Helper method called recursively to list child tasks.
        /// </summary>
        /// <param name="task">Task instance</param>
        /// <param name="indent">print indent</param>
        private static void listHierarchy(Task task, String indent)
        {
            foreach (Task child in ToEnumerable(task.getChildTasks()))
            {
                System.Console.WriteLine(indent + "Task: " + child.getName());
                listHierarchy(child, indent + " ");
            }
        }

        /// <summary>
        /// This method lists all resource assignments defined in the file.
        /// </summary>
        /// <param name="file">project file</param>
        private static void listAssignments(ProjectFile file)
        {
            Task task;
            Resource resource;
            String taskName;
            String resourceName;

            foreach (ResourceAssignment assignment in ToEnumerable(file.getAllResourceAssignments()))
            {
                task = assignment.getTask();
                if (task == null)
                {
                    taskName = "(null task)";
                }
                else
                {
                    taskName = task.getName();
                }

                resource = assignment.getResource();
                if (resource == null)
                {
                    resourceName = "(null resource)";
                }
                else
                {
                    resourceName = resource.getName();
                }

                System.Console.WriteLine("Assignment: Task=" + taskName + " Resource=" + resourceName);
            }

            System.Console.WriteLine();
        }

        /// <summary>
        /// This method displays the resource assignments for each task. 
        /// This time rather than just iterating through the list of all 
        /// assignments in the file, we extract the assignments on a task-by-task basis.
        /// </summary>
        /// <param name="file">project file</param>
        private static void listAssignmentsByTask(ProjectFile file)
        {
            foreach (Task task in ToEnumerable(file.getAllTasks()))
            {
                System.Console.WriteLine("Assignments for task " + task.getName() + ":");

                foreach (ResourceAssignment assignment in ToEnumerable(task.getResourceAssignments()))
                {
                    Resource resource = assignment.getResource();
                    String resourceName;

                    if (resource == null)
                    {
                        resourceName = "(null resource)";
                    }
                    else
                    {
                        resourceName = resource.getName();
                    }

                    System.Console.WriteLine("   " + resourceName);
                }
            }

            System.Console.WriteLine();
        }

        /// <summary>
        /// This method displays the resource assignments for each resource. 
        /// This time rather than just iterating through the list of all 
        /// assignments in the file, we extract the assignments on a resource-by-resource basis.
        /// </summary>
        /// <param name="file">project file</param>
        private static void listAssignmentsByResource(ProjectFile file)
        {
            foreach (Resource resource in ToEnumerable(file.getAllResources()))
            {
                System.Console.WriteLine("Assignments for resource " + resource.getName() + ":");

                foreach (ResourceAssignment assignment in ToEnumerable(resource.getTaskAssignments()))
                {
                    Task task = assignment.getTask();
                    System.Console.WriteLine("   " + task.getName());
                }
            }

            System.Console.WriteLine();
        }

        /// <summary>
        /// This method lists any notes attached to tasks..
        /// </summary>
        /// <param name="file">project file</param>
        private static void listTaskNotes(ProjectFile file)
        {
            foreach (Task task in ToEnumerable(file.getAllTasks()))
            {
                String notes = task.getNotes();

                if (notes != null && notes.Length != 0)
                {
                    System.Console.WriteLine("Notes for " + task.getName() + ": " + notes);
                }
            }

            System.Console.WriteLine();
        }

        /// <summary>
        /// This method lists any notes attached to resources.
        /// </summary>
        /// <param name="file">project file</param>
        private static void listResourceNotes(ProjectFile file)
        {
            foreach (Resource resource in ToEnumerable(file.getAllResources()))
            {
                String notes = resource.getNotes();

                if (notes != null && notes.Length != 0)
                {
                    System.Console.WriteLine("Notes for " + resource.getName() + ": " + notes);
                }
            }

            System.Console.WriteLine();
        }

        /// <summary>
        /// This method lists task predecessor and successor relationships.
        /// </summary>
        /// <param name="file">project file</param>
        private static void listRelationships(ProjectFile file)
        {
            foreach (Task task in ToEnumerable(file.getAllTasks()))
            {
                System.Console.Write(task.getID());
                System.Console.Write('\t');
                System.Console.Write(task.getName());
                System.Console.Write('\t');

                dumpRelationList(task.getPredecessors());
                System.Console.Write('\t');
                dumpRelationList(task.getSuccessors());
                System.Console.WriteLine();
            }
        }

        /// <summary>
        /// Internal utility to dump relationship lists in a structured format that can 
        /// easily be compared with the tabular data in MS Project.
        /// </summary>
        /// <param name="relations">project file</param>
        private static void dumpRelationList(java.util.List relations)
        {
            if (relations != null && relations.isEmpty() == false)
            {
                if (relations.size() > 1)
                {
                    System.Console.Write('"');
                }
                bool first = true;
                foreach (Relation relation in ToEnumerable(relations))
                {
                    if (!first)
                    {
                        System.Console.Write(',');
                    }
                    first = false;
                    System.Console.Write(relation.getTargetTask().getID());
                    Duration lag = relation.getLag();
                    if (relation.getType() != RelationType.FINISH_START || lag.getDuration() != 0)
                    {
                        System.Console.Write(relation.getType());
                    }

                    if (lag.getDuration() != 0)
                    {
                        if (lag.getDuration() > 0)
                        {
                            System.Console.Write("+");
                        }
                        System.Console.Write(lag);
                    }
                }
                if (relations.size() > 1)
                {
                    System.Console.Write('"');
                }
            }
        }

        /// <summary>
        /// List the slack values for each task.
        /// </summary>
        /// <param name="file">project file</param>
        private static void listSlack(ProjectFile file)
        {
            foreach (Task task in ToEnumerable(file.getAllTasks()))
            {
                System.Console.WriteLine(task.getName() + " Total Slack=" + task.getTotalSlack() + " Start Slack=" + task.getStartSlack() + " Finish Slack=" + task.getFinishSlack());
            }
        }

        /// <summary>
        /// List details of all calendars in the file.
        /// </summary>
        /// <param name="file">project file</param>
        private static void listCalendars(ProjectFile file)
        {
            foreach (ProjectCalendar cal in ToEnumerable(file.getBaseCalendars()))
            {
                System.Console.WriteLine(cal.toString());
            }

            foreach (ProjectCalendar cal in ToEnumerable(file.getResourceCalendars()))
            {
                System.Console.WriteLine(cal.toString());
            }
        }
    }
}