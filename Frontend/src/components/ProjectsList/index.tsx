import React, { Fragment } from "react";
import { useQuery } from "@apollo/react-hooks";
import Card from "../Card";
import gql from "graphql-tag";
import { Grid } from "@material-ui/core";
import { ProjectStatusFilter } from "../../api/types";
import Loader from "../Loader";

interface IProjectListProps {
  categoryId?: string;
  status?: string;
}

export default ({
  categoryId,
  status = ProjectStatusFilter.Active,
}: IProjectListProps) => {
  const query = categoryId
    ? useQuery(FIND_PROJECTS, {
        variables: {
          categoryId: categoryId,
          status: status,
        },
      })
    : useQuery(FIND_ALL_PROJECTS);

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
          data.projects.map((project, index) => (
            <Grid item xs={12} sm={6} md={4} key={index}>
              <Card project={project} />
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
  query FindProjects($categoryId: [String]) {
    projects(
      where: [{ path: "categoryId", comparison: equal, value: $categoryId }]
    ) {
      id
      name
      description
      url
      categoryId
      category {
        color
        colorHex
        name
      }
      descriptiveImage {
        filepath
        url
      }
      goal
      end
      target
      proposal
      remainingTime
      participantCounts {
        count
      }
      displayPriority
      isActive
      isComingSoon
      isClosed
    }
  }
`;

const FIND_ALL_PROJECTS = gql`
  query FindAllProjects {
    projects {
      id
      name
      description
      url
      categoryId
      category {
        color
        colorHex
        name
      }
      descriptiveImage {
        filepath
        url
      }
      goal
      end
      target
      proposal
      remainingTime
      participantCounts {
        count
      }
      displayPriority
      isActive
      isComingSoon
      isClosed
    }
  }
`;
