<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="5a3d6a66-fad1-4b4a-9be9-861a655f6d75" namespace="Andrique.Utils.X7Actions" xmlSchemaNamespace="urn:Andrique.Utils.X7Actions" assemblyName="Andrique.Utils.X7Actions" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="ConfigurationSection1" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="configurationSection1">
      <attributeProperties>
        <attributeProperty name="AttributeProperty1" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="attributeProperty1" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/5a3d6a66-fad1-4b4a-9be9-861a655f6d75/Int32" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationSection>
    <configurationElement name="ConfigurationElement1">
      <attributeProperties>
        <attributeProperty name="AttributeProperty1" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="attributeProperty1" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/5a3d6a66-fad1-4b4a-9be9-861a655f6d75/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="ConfigurationElementCollection1" xmlItemName="configurationElement1" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/5a3d6a66-fad1-4b4a-9be9-861a655f6d75/ConfigurationElement1" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>