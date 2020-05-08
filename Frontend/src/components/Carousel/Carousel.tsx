import React from "react";
import { gql, useQuery } from "@apollo/client";
import Slider from "react-slick";
import { Grid, Container } from "@material-ui/core";


import { Fragments } from "../../api/fragments";
import { ICrowdaction } from "../../api/types";

import Loader from "../Loader/Loader";
import CrowdactionCard from "../CrowdactionCard/CrowdactionCard";
import { SecondaryButton, SecondaryGhostButton } from "../Button/Button";

import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

import styles from "./Carousel.module.scss";
import { Alert } from "../Alert/Alert";

interface ICarouselProps {
  title: string;
  text: string;
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

  const { data, loading, error } = useQuery(GET_CAROUSEL_CROWDACTIONS);

  if (error) {
    console.error(error.message);
    return <Alert type="error" text={error.message} />;
  }

  return (
    <Container>
      <Grid container className={styles.container}>
        <Grid item sm={12} md={3}>
          <div className={styles.intro}>
            <h2 className={styles.introTitle}>{ title }</h2>
            <p
              className={styles.introText}
              dangerouslySetInnerHTML={{ __html: text }}
            ></p>
            <SecondaryButton to="crowdactions/find">
              Find Crowdaction
            </SecondaryButton>
            <SecondaryGhostButton to="crowdactions/start">
              Start Crowdaction
            </SecondaryGhostButton>
          </div>
        </Grid>
        <Grid item sm={12} md={9} className={styles.sliderContainer}>
          {loading && <Loader />}
          {data?.crowdactions && (
            <Slider {...settings}>
              {data.crowdactions.map((crowdaction: ICrowdaction, index: number) => (
                <CrowdactionCard crowdaction={crowdaction} key={index} />
              ))}
            </Slider>
          )}
        </Grid>
      </Grid>
    </Container>
  );
};

const GET_CAROUSEL_CROWDACTIONS = gql`
  query GetCarouselCrowdactions {
    crowdactions(take: 6, status: ACTIVE) {
      ${Fragments.crowdactionDetail}
    }
  }
`;
