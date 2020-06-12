import React from "react";
import { GhostButton } from "../../components/Button/Button";
import Share from "../../components/Share/Share";
import { Section } from "../../components/Section/Section";
import Carousel from "../../components/Carousel/Carousel";
import CrowdactionsList from "../../components/CrowdactionsList/CrowdactionsList";
import Stats from "../../components/Stats/Stats";
import TimeToAct from "../../components/TimeToAct/TimeToAct";

import { useTranslation } from 'react-i18next';
import { Helmet } from "react-helmet";
import { gql, useQuery } from "@apollo/client";
import InstagramWall from "../../components/InstagramWall/InstagramWall";
import { Alert } from "../../components/Alert/Alert";
import { IInstagramWallItem } from "../../api/types";
import Loader from "../../components/Loader/Loader";

const GET_INSTAGRAM_WALL = gql`
  query GetInstagramWall($user: String!) {
    instagramWall(user: $user) {
      id
      shortCode
      thumbnailSrc
      caption
      accessibilityCaption
      link      
      date
    }
  }
`;

const HomePage = () => {
  const { t } = useTranslation();
  const { data, error, loading } = useQuery(
    GET_INSTAGRAM_WALL,
    {
      variables: {
        user: "collaction_org"
      }
    }
  );
  const wallData = data?.instagramWall as IInstagramWallItem[] | null;
  return (
    <>
      <Helmet>
        <title>CollAction</title>
        <meta name="description" content="CollAction" />
      </Helmet>
      <Carousel title={t('home.carousel.title')} text={t('home.carousel.text')} />
      <Section center color="grey" title={t('home.intro.title')}>
        <p dangerouslySetInnerHTML={{ __html: t('home.intro.text') }} />
        <GhostButton to="/about">Learn more</GhostButton>
      </Section>
      <Section center title={t('home.timeToAct.title')}>
        <TimeToAct />
      </Section>
      <Section center color="grey" title={t('home.stats.title')}>
        <Stats />
        <Share />
      </Section>
      <Section center title={t('home.crowdactions.title')}>
        <CrowdactionsList />
        <div style={{marginTop: 20 }}>
          <GhostButton to="/crowdactions/find">{t('home.crowdactions.button')}</GhostButton>
        </div>
      </Section>
        <Section center title={t('home.follow.title')}>
            Instagram: <a href="https://www.instagram.com/collaction_org" target="_blank" rel="noopener noreferrer" >@collaction_org</a>
            <Alert type="error" text={error?.message} />
            { loading && <Loader /> }
            { wallData && <InstagramWall wallItems={wallData} /> }
        </Section>
    </>
  )
}

export default HomePage;