import React, { Fragment } from "react";
import { useQuery } from "@apollo/react-hooks";
import Card from "../Card";
import gql from "graphql-tag";
import { Grid } from "@material-ui/core";
import { ProjectStatusFilter } from "../../api/types";

export default ({
  categoryId,
  status,
}: {
  categoryId: string;
  status: string;
}) => {
  const query = categoryId
    ? useQuery(FIND_PROJECTS, {
        variables: {
          categoryId: categoryId,
          isActive: `${status === ProjectStatusFilter.Active}`,
          isClosed: `${status === ProjectStatusFilter.Closed}`,
          isComingSoon: `${status === ProjectStatusFilter.ComingSoon}`,
        },
      })
    : useQuery(GET_PROJECTS);

  const { data, loading, error } = query;

  if (loading) {
    return <div>Loading projects...</div>;
  }

  if (error) {
    console.error(error);
    return;
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

const GET_PROJECTS = gql`
  query {
    projects {
      id
      description
      name
      url
      category {
        colorHex
        name
      }
      descriptiveImage {
        filepath
        url
      }
      goal
      end
      remainingTime
      target
      participantCounts {
        count
      }
      status
      url
    }
  }
`;

const FIND_PROJECTS = gql`
  query FindProjects(
    $categoryId: [String]
    $isActive: String = "true"
    $isClosed: String = "false"
    $isComingSoon: String = "false"
  ) {
    projects(
      where: [
        { path: "categoryId", comparison: equal, value: $categoryId }
        { path: "isActive", comparison: equal, value: [$isActive] }
        { path: "isClosed", comparison: equal, value: [$isClosed] }
        { path: "isComingSoon", comparison: equal, value: [$isComingSoon] }
      ]
    ) {
      id
      name
      description
      url
      category {
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
      remainingTime
      participantCounts {
        count
      }
      status
    }
  }
`;
