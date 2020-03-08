import React from "react";
import { useQuery } from "@apollo/react-hooks";
import { gql } from "apollo-boost";
import Slider from "react-slick";
import { Grid, Container } from "@material-ui/core";
import Card from "../Card";
import Loader from "../Loader";
import { SecondaryButton, SecondaryGhostButton } from "../Button/Button";
import { Fragments } from "../../api/fragments";

import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import styles from "./Carousel.module.scss";
import { IProject } from "../../api/types";

interface ICarouselProps {
  title?: string;
  text?: any;
}

export default ({ title, text }: ICarouselProps) => {
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

  if (error || !data) {
    console.error(error);
    return null;
  }

  return (
    <Container>
      <Grid container className={styles.container}>
        <Grid item sm={12} md={3}>
          <div className={styles.intro}>
            <h2 className={styles.introTitle}>{title}</h2>
            <p
              className={styles.introText}
              dangerouslySetInnerHTML={{ __html: text }}
            ></p>
            <SecondaryButton to="projects/find">
              Find Crowdaction
            </SecondaryButton>
            <SecondaryGhostButton to="projects/start">
              Start Crowdaction
            </SecondaryGhostButton>
          </div>
        </Grid>
        <Grid item sm={12} md={9} className={styles.sliderContainer}>
          {loading && <Loader />}
          {data.projects && (
            <Slider {...settings}>
              {data.projects.map((project: IProject, index: number) => (
                <Card project={project} key={index} />
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
      ${Fragments.projectDetail}
    }
  }
`;
