import React, { Fragment } from "react";
import Slider from "react-slick";
import "./style.module.scss";
import { Grid, Container } from "@material-ui/core";
import Card from "../Card";
import { useQuery } from "react-apollo";
import Loader from "../Loader";
import gql from "graphql-tag";

export default () => {
  const settings = {
    dots: false,
    infinite: false,
    arrows: true,
    speed: 500,
    slidesToShow: 3,
    slidesToScroll: 1,
  };

  const query = useQuery(GET_CAROUSEL_PROJECTS);
  const { data, loading, error } = query;

  if (loading) {
    return <Loader />;
  }

  if (error) {
    console.error(error);
    return;
  }

  if (!data.projects || !data.projects.length) {
    return <div>No projects.</div>;
  }

  return (
    <Container>
      <Slider {...settings}>
        {data.projects.map((project, index) => (
          <Grid item xs={12} sm={6} md={4} key={index}>
            <Card project={project} />
          </Grid>
        ))}
      </Slider>
    </Container>
  );
};

const GET_CAROUSEL_PROJECTS = gql`
  query GetCarouselProjects {
    projects(take: 6) {
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
