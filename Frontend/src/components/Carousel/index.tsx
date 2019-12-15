import React from "react";
import { useQuery } from "react-apollo";
import gql from "graphql-tag";
import Slider from "react-slick";
import { Grid, Container } from "@material-ui/core";
import Card from "../Card";
import Loader from "../Loader";
import { SecondaryButton, GhostButton } from "../Button";

import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import styles from "./style.module.scss";

export default () => {
  const settings = {
    dots: false,
    infinite: true,
    arrows: true,
    speed: 500,
    slidesToScroll: 1,
    variableWidth: true,
    responsive: [
      {
        breakpoint: 600,
        settings: {
          arrows: false,
        },
      },
    ],
  };

  const query = useQuery(GET_CAROUSEL_PROJECTS);
  const { data, loading, error } = query;

  if (error) {
    console.error(error);
    return;
  }

  return (
    <Container>
      <Grid container className={styles.container}>
        <Grid item sm={12} md={3}>
          <div className={styles.intro}>
            <h1 className={styles.introTitle}>Power to the crowd</h1>
            <p>
              We help people solve collective action problems through
              crowdacting.
            </p>
            <SecondaryButton to="projects/find">
              Find Crowdaction
            </SecondaryButton>
            <GhostButton to="projects/start">Start Crowdaction</GhostButton>
          </div>
        </Grid>
        <Grid item sm={12} md={9} className={styles.sliderContainer}>
          {loading && <Loader />}
          {data.projects && (
            <Slider {...settings}>
              {data.projects.map((project, index) => (
                // <Grid item xs={12} sm={6} md={4} key={index}>
                <Card project={project} key={index} />
                // </Grid>
              ))}
            </Slider>
          )}
        </Grid>
      </Grid>
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
