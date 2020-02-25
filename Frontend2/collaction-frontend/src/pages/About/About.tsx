import React from 'react';
import { Grid } from '@material-ui/core';

import styles from './About.module.scss';
import { Section } from '../../components/Section';
import { Faq } from '../../components/Faq';

import { useTranslation } from 'react-i18next';

const AboutPage = () => {
  const { t } = useTranslation();

  const videos = {
    mainvideo: ''
  };

  const mission = {
    html: ''
  };

  const about = {
    html: ''
  };

  const join = {
    html: ''
  };

  const partners = {
    html: ''
  };

  const meetTheTeam = {
    title: 'Meet the team',
    team: [
      {
        name: 'Testname',
        photo: ''
      }
    ]
  };

  const faqs = [
    {
      name: '',
      html: ''
    }
  ];

  const generateMemberPhoto = (member: any) => (
    <li key={member.name} className={styles.teamMember}>
      <img src={member.photo} alt={member.name} title={member.name} />
      <span>{member.name}</span>
    </li>
  );

  return (
    <React.Fragment>
      <Grid className={styles.video}>
        <iframe
          title="Collective actions"
          src={videos.mainvideo}
          frameBorder="0"
          allowFullScreen
        ></iframe>
      </Grid>
      <Section color="green">
        <h2>{t('about.mission.title')}</h2>
        <span dangerouslySetInnerHTML={{ __html: t('about.mission.html') }}></span>
      </Section>
      <Section>
        <span dangerouslySetInnerHTML={{ __html: about.html }}></span>
      </Section>
      <Section color="grey" title={meetTheTeam.title}>
        <ul className={styles.team}>
          {meetTheTeam.team.map(generateMemberPhoto)}
        </ul>
      </Section>
      <Section>
        <span dangerouslySetInnerHTML={{ __html: join.html }}></span>
      </Section>
      <Section color="grey">
        <span dangerouslySetInnerHTML={{ __html: partners.html }}></span>
      </Section>
      <Section title="Frequently Asked Questions">
        {faqs.map((faq) => (
          <Faq key={faq.name} title={faq.name} content={faq.html}></Faq>
        ))}
      </Section>
    </React.Fragment>
  );
};

export default AboutPage;
