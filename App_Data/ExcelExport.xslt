<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

  <xsl:template match="root">
    <xsl:element name="sheetData">
      <xsl:apply-templates select="columns"></xsl:apply-templates>
      <xsl:apply-templates select="data"></xsl:apply-templates>
    </xsl:element>
  </xsl:template>

  <xsl:template match="columns">
    <xsl:variable name="rowID">
      <xsl:number value="position()" format="1"/>
    </xsl:variable>

    <xsl:element name="row">
      <xsl:attribute name="r">
        <xsl:value-of select="$rowID"/>
      </xsl:attribute>

      <xsl:attribute name="ht">
        <xsl:value-of select="25"/>
      </xsl:attribute>

      <xsl:attribute name="customHeight">
        <xsl:value-of select="1"/>
      </xsl:attribute>

      <xsl:for-each select="*">
        <xsl:element name="c">
          <xsl:variable name="colID">
            <xsl:number value="position()" format="A"/>
          </xsl:variable>
          <xsl:attribute name="r">
            <xsl:value-of  select="concat(string($colID),string($rowID))"/>
          </xsl:attribute>
          <xsl:attribute name="t">
            <xsl:text>inlineStr</xsl:text>
          </xsl:attribute>
          <xsl:element name="is">
            <xsl:element name="t">
              <xsl:value-of select="."/>
            </xsl:element>
          </xsl:element>
        </xsl:element>
      </xsl:for-each>

    </xsl:element>
  </xsl:template>

  <xsl:template match="data">
    <xsl:variable name="rowID">
      <xsl:number value="position() + 1" format="1"/>
    </xsl:variable>
    <xsl:element name="row">
      <xsl:attribute name="r">
        <xsl:value-of select="$rowID"/>
      </xsl:attribute>

      <xsl:for-each select="*">
        <xsl:element name="c">
          <xsl:variable name="colID">
            <xsl:number value="position()" format="A"/>
          </xsl:variable>
          <xsl:attribute name="r">
            <xsl:value-of  select="concat(string($colID),string($rowID))"/>
          </xsl:attribute>
          <xsl:attribute name="t">
            <xsl:text>inlineStr</xsl:text>
          </xsl:attribute>
          <xsl:element name="is">
            <xsl:element name="t">
              <xsl:value-of select="."/>
            </xsl:element>
          </xsl:element>
        </xsl:element>
      </xsl:for-each>

    </xsl:element>
  </xsl:template>

</xsl:stylesheet>

