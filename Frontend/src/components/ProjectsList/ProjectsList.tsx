import React, { Fragment } from "react";
import { gql, useQuery } from "@apollo/client";
import { Fragments } from "../../api/fragments";
import { Grid } from "@material-ui/core";

import ProjectCard from "../ProjectCard";
import { ProjectStatusFilter, IProject } from "../../api/types";

import Loader from "../Loader/Loader";

interface IProjectListProps {
  category?: string;
  status?: string;
}

export default ({
  category,
  status = ProjectStatusFilter.Active,
}: IProjectListProps) => {
  const query = useQuery(
    FIND_PROJECTS,
    category
      ? {
          variables: {
            category: category,
            status: status,
          },
        }
      : {
          variables: {
            status: status,
          },
        }
  );

  const { data, loading, error } = query;

  if (loading) {
    return <Loader />;
  }

  if (error) {
    console.error(error);
    return null;
  }

  return (
    <Fragment>
      <Grid container spacing={3}>
        {data.projects && data.projects.length ? (
          data.projects.map((project: IProject, index: number) => (
            <Grid item xs={12} sm={6} md={4} key={index}>
              <ProjectCard project={project} />
            </Grid>
          ))
        ) : (
          <div>No projects here yet.</div>
        )}
      </Grid>
    </Fragment>
  );
};

const FIND_PROJECTS = gql`
  query FindProjects($category: Category, $status: SearchProjectStatus) {
    projects(category: $category, status: $status) {
      ${Fragments.projectDetail}
    }
  }
`;
